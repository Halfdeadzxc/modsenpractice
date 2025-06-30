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
    public class RepostRepositoryTests
    {
        private async Task<AppDbContext> CreateContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "reposter@example.com",
                Username = "reposter",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token",
                CreatedAt = DateTime.UtcNow
            };

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Content = "Test post",
                AuthorId = user.Id,
                Author = user,
                Hashtags = "#repost",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddRepost()
        {
            using var context = await CreateContextAsync();
            var repo = new RepostRepository(context);
            var user = context.Users.First();
            var post = context.Posts.First();

            var repost = new Repost
            {
                UserId = user.Id,
                PostId = post.Id,
                Comment = "Cool post!"
            };

            var result = await repo.AddAsync(repost);

            Assert.NotNull(result);
            Assert.Single(context.Reposts);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task GetRepostsAsync_ShouldReturnPosts()
        {
            using var context = await CreateContextAsync();
            var repo = new RepostRepository(context);
            var user = context.Users.First();
            var post = context.Posts.First();

            await context.Reposts.AddAsync(new Repost
            {
                UserId = user.Id,
                PostId = post.Id,
                Comment = "Shared"
            });
            await context.SaveChangesAsync();

            var results = await repo.GetRepostsAsync(post.Id);

            Assert.Single(results);
            Assert.Equal(post.Id, results.First().Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRepost()
        {
            using var context = await CreateContextAsync();
            var repo = new RepostRepository(context);
            var user = context.Users.First();
            var post = context.Posts.First();

            await context.Reposts.AddAsync(new Repost
            {
                UserId = user.Id,
                PostId = post.Id,
                Comment = "Find me"
            });
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(user.Id, post.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueIfRepostExists()
        {
            using var context = await CreateContextAsync();
            var repo = new RepostRepository(context);
            var user = context.Users.First();
            var post = context.Posts.First();

            await context.Reposts.AddAsync(new Repost
            {
                UserId = user.Id,
                PostId = post.Id,
                Comment = string.Empty
            });
            await context.SaveChangesAsync();

            var exists = await repo.ExistsAsync(user.Id, post.Id);

            Assert.True(exists);
        }
    }
}
