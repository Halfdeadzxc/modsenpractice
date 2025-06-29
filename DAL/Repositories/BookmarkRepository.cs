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
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly AppDbContext _context;

        public BookmarkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Bookmark> AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
        {
            await _context.Bookmarks.AddAsync(bookmark, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return bookmark;
        }

        public async Task DeleteAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId, cancellationToken);

            if (bookmark is not null)
            {
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Post>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Select(b => b.Post)
                .ToListAsync(cancellationToken);
        }

        public async Task<Bookmark> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .AnyAsync(b => b.UserId == userId && b.PostId == postId, cancellationToken);
        }

    }
}
