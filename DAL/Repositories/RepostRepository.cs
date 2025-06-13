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
    public class RepostRepository : IRepostRepository
    {
        private readonly AppDbContext _context;

        public RepostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRepostAsync(Guid userId, Guid postId, string comment)
        {
            var repost = new Repost { UserId = userId, PostId = postId, Comment = comment };
            _context.Reposts.Add(repost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Post>> GetRepostsAsync(Guid postId)
        {
            return await _context.Reposts.Where(r => r.PostId == postId)
                                         .Select(r => r.Post)
                                         .ToListAsync();
        }
    }
}
