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

        public async Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default)
        {
            await _context.Subscriptions.AddAsync(subscription, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return subscription;
        }

        public async Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.FollowerId == followerId && s.FollowingId == followingId, cancellationToken);

            if (subscription is not null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .AsNoTracking()
                .Where(s => s.FollowingId == userId)
                .Select(s => s.Follower)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .AsNoTracking()
                .Where(s => s.FollowerId == userId)
                .Select(s => s.Following)
                .ToListAsync(cancellationToken);
        }

        public async Task<Subscription> GetByIdAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.FollowerId == followerId && s.FollowingId == followingId, cancellationToken);
        }

    }
}
