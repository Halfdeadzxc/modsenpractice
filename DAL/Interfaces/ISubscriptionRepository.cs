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
        Task<bool> SubscribeAsync(Guid followerId, Guid followingId);
        Task<bool> UnsubscribeAsync(Guid followerId, Guid followingId);
        Task<IEnumerable<User>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<User>> GetFollowingAsync(Guid userId);
    }
}
