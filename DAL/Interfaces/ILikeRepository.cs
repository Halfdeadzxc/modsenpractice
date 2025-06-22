using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ILikeRepository
    {
        Task<Like> AddAsync(Like like, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetLikesAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<Like> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
    }
}
