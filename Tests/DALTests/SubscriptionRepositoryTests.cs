using DAL.Models;
using DAL.Repositories;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DALTests
{
    public class SubscriptionRepositoryTests
    {
        private async Task<AppDbContext> CreateContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var follower = new User
            {
                Id = Guid.NewGuid(),
                Username = "follower",
                Email = "follower@example.com",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token",
                CreatedAt = DateTime.UtcNow
            };

            var following = new User
            {
                Id = Guid.NewGuid(),
                Username = "following",
                Email = "following@example.com",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token2",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddRangeAsync(follower, following);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddSubscription()
        {
            using var context = await CreateContextAsync();
            var repo = new SubscriptionRepository(context);
            var follower = context.Users.First();
            var following = context.Users.Last();

            var subscription = new Subscription
            {
                FollowerId = follower.Id,
                FollowingId = following.Id,
                Status = "approved"
            };

            var result = await repo.AddAsync(subscription);

            Assert.NotNull(result);
            Assert.Single(context.Subscriptions);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveSubscription()
        {
            using var context = await CreateContextAsync();
            var repo = new SubscriptionRepository(context);
            var follower = context.Users.First();
            var following = context.Users.Last();

            await context.Subscriptions.AddAsync(new Subscription
            {
                FollowerId = follower.Id,
                FollowingId = following.Id,
                Status = "approved"
            });
            await context.SaveChangesAsync();

            await repo.DeleteAsync(follower.Id, following.Id);
            Assert.Empty(context.Subscriptions);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSubscription()
        {
            using var context = await CreateContextAsync();
            var repo = new SubscriptionRepository(context);
            var follower = context.Users.First();
            var following = context.Users.Last();

            var subscription = new Subscription
            {
                FollowerId = follower.Id,
                FollowingId = following.Id,
                Status = "approved"
            };

            await context.Subscriptions.AddAsync(subscription);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(follower.Id, following.Id);

            Assert.NotNull(result);
            Assert.Equal(follower.Id, result.FollowerId);
        }

        [Fact]
        public async Task GetFollowersAsync_ShouldReturnFollower()
        {
            using var context = await CreateContextAsync();
            var repo = new SubscriptionRepository(context);
            var follower = context.Users.First();
            var following = context.Users.Last();

            await context.Subscriptions.AddAsync(new Subscription
            {
                FollowerId = follower.Id,
                Follower = follower,
                FollowingId = following.Id,
                Following = following,
                Status = "approved"
            });
            await context.SaveChangesAsync();

            var result = await repo.GetFollowersAsync(following.Id);

            Assert.Single(result);
            Assert.Equal(follower.Id, result.First().Id);
        }

        [Fact]
        public async Task GetFollowingAsync_ShouldReturnFollowing()
        {
            using var context = await CreateContextAsync();
            var repo = new SubscriptionRepository(context);
            var follower = context.Users.First();
            var following = context.Users.Last();

            await context.Subscriptions.AddAsync(new Subscription
            {
                FollowerId = follower.Id,
                Follower = follower,
                FollowingId = following.Id,
                Following = following,
                Status = "approved"
            });
            await context.SaveChangesAsync();

            var result = await repo.GetFollowingAsync(follower.Id);

            Assert.Single(result);
            Assert.Equal(following.Id, result.First().Id);
        }
    }
}
