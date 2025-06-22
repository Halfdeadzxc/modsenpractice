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

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Post> GetByIdAsync(Guid postId)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetByUserAsync(Guid userId)
        {
            return await _context.Posts.Where(p => p.AuthorId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetByHashtagAsync(string hashtag)
        {
            return await _context.Posts.Where(p => p.Hashtags.Contains(hashtag)).ToListAsync();
        }
        public async Task IncrementLikeCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.LikeCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementLikeCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.LikeCount > 0)
            {
                post.LikeCount--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementCommentCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.CommentCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementCommentCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.CommentCount > 0)
            {
                post.CommentCount--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementRepostCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.RepostCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementBookmarkCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.BookmarkCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementBookmarkCountAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.BookmarkCount > 0)
            {
                post.BookmarkCount--;
                await _context.SaveChangesAsync();
            }
        }
    }
}
