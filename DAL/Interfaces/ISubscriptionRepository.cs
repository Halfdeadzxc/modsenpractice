using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Subscription> GetByIdAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
    }
}
