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
        Task<Bookmark> AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Bookmark> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
    }
}
