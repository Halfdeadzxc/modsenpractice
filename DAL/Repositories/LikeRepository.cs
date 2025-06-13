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
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext _context;

        public LikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddLikeAsync(Guid userId, Guid postId)
        {
            var like = new Like { UserId = userId, PostId = postId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveLikeAsync(Guid userId, Guid postId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like == null) return false;

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetLikesAsync(Guid postId)
        {
            return await _context.Likes.Where(l => l.PostId == postId)
                                       .Select(l => l.User)
                                       .ToListAsync();
        }
    }
}
