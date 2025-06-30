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
    public class PostRepositoryTests
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
                Email = "user@example.com",
                Username = "user",
                PasswordHash = "hashed",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return context;
        }

        private static Post CreatePost(User user) => new Post
        {
            Id = Guid.NewGuid(),
            Content = "Test content",
            Hashtags = "#test",
            AuthorId = user.Id,
            Author = user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task AddAsync_ShouldAddPost()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();

            var post = CreatePost(user);
            var result = await repo.AddAsync(post);

            Assert.NotNull(result);
            Assert.Single(context.Posts);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyPostContent()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();

            var post = await repo.AddAsync(CreatePost(user));
            post.Content = "Updated content";

            var updated = await repo.UpdateAsync(post);

            Assert.Equal("Updated content", updated.Content);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePost()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();

            var post = await repo.AddAsync(CreatePost(user));
            await repo.DeleteAsync(post.Id);

            Assert.Empty(context.Posts);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPost()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();
            var post = await repo.AddAsync(CreatePost(user));

            var found = await repo.GetByIdAsync(post.Id);

            Assert.NotNull(found);
            Assert.Equal(post.Content, found.Content);
        }

        [Fact]
        public async Task GetByUserAsync_ShouldReturnUserPosts()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();
            await repo.AddAsync(CreatePost(user));

            var posts = await repo.GetByUserAsync(user.Id);

            Assert.Single(posts);
        }

        [Fact]
        public async Task GetByHashtagAsync_ShouldReturnMatchingPosts()
        {
            using var context = await CreateContextAsync();
            var repo = new PostRepository(context);
            var user = context.Users.First();
            var post = CreatePost(user);
            post.Hashtags = "#foo #bar";

            await repo.AddAsync(post);

            var result = await repo.GetByHashtagAsync("#bar");

            Assert.Single(result);
        }
    }
}
