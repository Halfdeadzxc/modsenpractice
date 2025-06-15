using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly AppDbContext _context;

        public FeedRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetFeedAsync(Guid userId, int page, int pageSize, string filter)
        {
            var query = _context.Posts.AsQueryable();

            if (filter == "subscriptions")
            {
                var followingIds = await _context.Subscriptions
                    .Where(s => s.FollowerId == userId)
                    .Select(s => s.FollowingId)
                    .ToListAsync();

                query = query.Where(p => followingIds.Contains(p.AuthorId));
            }

            else if (filter == "popular")
            {
                query = query.OrderByDescending(p => p.LikeCount);
            }

            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }

}
