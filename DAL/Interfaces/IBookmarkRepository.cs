using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<bool> AddBookmarkAsync(Guid userId, Guid postId);
        Task<bool> RemoveBookmarkAsync(Guid userId, Guid postId);
        Task<IEnumerable<Post>> GetBookmarksAsync(Guid userId);
        Task<Bookmark> GetBookmarkAsync(Guid userId, Guid postId);
        Task<bool> ExistsAsync(Guid userId, Guid postId);
    }
}
