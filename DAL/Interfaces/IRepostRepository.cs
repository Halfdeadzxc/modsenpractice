using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepostRepository
    {
        Task<Repost> AddAsync(Repost repost, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetRepostsAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<Repost> GetByIdAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
    }
}
