using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid commentId);
        Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId);
        Task<Comment> GetByIdAsync(Guid commentId);
    }
}
