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

        public async Task<Repost> AddAsync(Repost repost, CancellationToken cancellationToken = default)
        {
            await _context.Reposts.AddAsync(repost, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return repost;
        }

        public async Task<IEnumerable<Post>> GetRepostsAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Reposts
                .Where(r => r.PostId == postId)
                .Select(r => r.Post)
                .ToListAsync(cancellationToken);
        }

        public async Task<Repost> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Reposts
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == postId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Reposts
                .AnyAsync(r => r.UserId == userId && r.PostId == postId, cancellationToken);
        }
    }
}
