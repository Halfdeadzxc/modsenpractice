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
    public class LikeRepositoryTests
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
                Email = "likeuser@example.com",
                Username = "likeuser",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token",
                CreatedAt = DateTime.UtcNow
            };

            var post = new Post
            {
                Id = Guid.NewGuid(),
                AuthorId = user.Id,
                Content = "Test post",
                Hashtags = "#like",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Author = user
            };

            await context.Users.AddAsync(user);
            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddLike()
        {
            using var context = await CreateContextAsync();
            var repo = new LikeRepository(context);

            var user = context.Users.First();
            var post = context.Posts.First();
            var like = new Like { UserId = user.Id, PostId = post.Id };

            var result = await repo.AddAsync(like);

            Assert.NotNull(result);
            Assert.Single(context.Likes);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveLike()
        {
            using var context = await CreateContextAsync();
            var repo = new LikeRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;
            await context.Likes.AddAsync(new Like { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            await repo.DeleteAsync(userId, postId);
            Assert.Empty(context.Likes);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLike()
        {
            using var context = await CreateContextAsync();
            var repo = new LikeRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;
            await context.Likes.AddAsync(new Like { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(userId, postId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueIfExists()
        {
            using var context = await CreateContextAsync();
            var repo = new LikeRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;
            await context.Likes.AddAsync(new Like { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            var exists = await repo.ExistsAsync(userId, postId);
            Assert.True(exists);
        }

        [Fact]
        public async Task GetLikesAsync_ShouldReturnUsers()
        {
            using var context = await CreateContextAsync();
            var repo = new LikeRepository(context);

            var user = context.Users.First();
            var post = context.Posts.First();
            await context.Likes.AddAsync(new Like { UserId = user.Id, PostId = post.Id });
            await context.SaveChangesAsync();

            var result = await repo.GetLikesAsync(post.Id);
            Assert.Single(result);
            Assert.Equal(user.Id, result.First().Id);
        }
    }
}
