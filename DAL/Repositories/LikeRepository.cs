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

        public async Task<Like> AddAsync(Like like, CancellationToken cancellationToken = default)
        {
            await _context.Likes.AddAsync(like, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return like;
        }

        public async Task DeleteAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<User>> GetLikesAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Likes
                .Where(l => l.PostId == postId)
                .Select(l => l.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<Like> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);
        }
    }
}
