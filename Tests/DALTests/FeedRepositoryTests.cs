using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.DALTests
{
    public class FeedRepositoryTests
    {
        private async Task<AppDbContext> CreateContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var author1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "author1@example.com",
                Username = "author1",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token",
                CreatedAt = DateTime.UtcNow
            };

            var author2 = new User
            {
                Id = Guid.NewGuid(),
                Email = "author2@example.com",
                Username = "author2",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token2",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddRangeAsync(author1, author2);

            var posts = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Content = "Post by author1",
                    AuthorId = author1.Id,
                    Hashtags = "#test",
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                    LikeCount = 5,
                    CommentCount = 2
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Content = "Post by author2",
                    AuthorId = author2.Id,
                    Hashtags = "#example",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2),
                    LikeCount = 3,
                    CommentCount = 5
                }
            };

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task GetPostsByAuthorsAsync_ShouldReturnFilteredPosts()
        {
            using var context = await CreateContextAsync();
            var repo = new FeedRepository(context);

            var authorId = context.Users.First().Id;
            var result = await repo.GetPostsByAuthorsAsync(new[] { authorId }, page: 1, pageSize: 10);

            Assert.Single(result);
            Assert.All(result, post => Assert.Equal(authorId, post.AuthorId));
        }

        [Fact]
        public async Task GetPopularPostsAsync_ShouldOrderByPopularity()
        {
            using var context = await CreateContextAsync();
            var repo = new FeedRepository(context);

            var since = DateTime.UtcNow.AddDays(-1);
            var result = (await repo.GetPopularPostsAsync(since, 1, 10)).ToList();

            Assert.Equal(2, result.Count);
            var score1 = result[0].LikeCount + result[0].CommentCount * 2;
            var score2 = result[1].LikeCount + result[1].CommentCount * 2;
            Assert.True(score1 >= score2);
        }

        [Fact]
        public async Task GetRecentPostsAsync_ShouldOrderByCreatedAtDescending()
        {
            using var context = await CreateContextAsync();
            var repo = new FeedRepository(context);

            var result = (await repo.GetRecentPostsAsync(1, 10)).ToList();

            Assert.Equal(2, result.Count);
            Assert.True(result[0].CreatedAt >= result[1].CreatedAt);
        }
    }
}
