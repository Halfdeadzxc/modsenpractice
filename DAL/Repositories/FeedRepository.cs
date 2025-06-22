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

        public async Task<IEnumerable<Post>> GetPostsByAuthorsAsync(IEnumerable<Guid> authorIds, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Where(p => authorIds.Contains(p.AuthorId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Post>> GetPopularPostsAsync(DateTime since, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Where(p => p.CreatedAt >= since)
                .OrderByDescending(p => p.LikeCount + p.CommentCount * 2)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }

}
