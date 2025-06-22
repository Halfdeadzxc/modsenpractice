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

        public async Task<bool> AddBookmarkAsync(Guid userId, Guid postId)
        {
            var bookmark = new Bookmark { UserId = userId, PostId = postId };
            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveBookmarkAsync(Guid userId, Guid postId)
        {
            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);
            if (bookmark is null) return false;

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Post>> GetBookmarksAsync(Guid userId)
        {
            return await _context.Bookmarks.Where(b => b.UserId == userId)
                                           .Select(b => b.Post)
                                           .ToListAsync();
        }

        public async Task<Bookmark> GetBookmarkAsync(Guid userId, Guid postId)
        {
            return await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid postId)
        {
            return await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.PostId == postId);
        }
    }
}
