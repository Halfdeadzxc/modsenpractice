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
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Post> AddAsync(Post post, CancellationToken cancellationToken = default)
        {
            await _context.Posts.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return post;
        }

        public async Task<Post> UpdateAsync(Post post, CancellationToken cancellationToken = default)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync(cancellationToken);
            return post;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var post = await _context.Posts.FindAsync(new object[] { id }, cancellationToken);
            if (post is not null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Post> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Post>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .AsNoTracking()
                .Where(p => p.AuthorId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Post>> GetByHashtagAsync(string hashtag, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .AsNoTracking()
                .Where(p => p.Hashtags.Contains(hashtag))
                .ToListAsync(cancellationToken);
        }

    }
}
