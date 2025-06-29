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
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            await _context.Comments.AddAsync(comment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return comment;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments.FindAsync(new object[] { id }, cancellationToken);
            if (comment is not null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId)
                .Include(c => c.Author)
                .ToListAsync(cancellationToken);
        }

        public async Task<Comment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }


        public async Task<Comment> UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return comment;
        }
    }
}
