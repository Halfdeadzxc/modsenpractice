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
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SubscribeAsync(Guid followerId, Guid followingId)
        {
            var subscription = new Subscription { FollowerId = followerId, FollowingId = followingId, Status = "approved" };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnsubscribeAsync(Guid followerId, Guid followingId)
        {
            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.FollowerId == followerId && s.FollowingId == followingId);
            if (subscription == null) return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId)
        {
            return await _context.Subscriptions.Where(s => s.FollowingId == userId)
                                               .Select(s => s.Follower)
                                               .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetFollowingAsync(Guid userId)
        {
            return await _context.Subscriptions.Where(s => s.FollowerId == userId)
                                               .Select(s => s.Following)
                                               .ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionAsync(Guid followerId, Guid followingId)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s =>
                    s.FollowerId == followerId &&
                    s.FollowingId == followingId);
        }
    }
}
