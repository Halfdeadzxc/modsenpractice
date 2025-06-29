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
    public class CommentRepositoryTests
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
                Email = "commenter@example.com",
                Username = "Commenter",
                PasswordHash = "hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "token"
            };

            var post = new Post
            {
                Id = Guid.NewGuid(),
                AuthorId = user.Id,
                Content = "Test Post",
                Hashtags = "#unit",
                MediaUrls = new(),
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
        public async Task AddAsync_ShouldAddComment()
        {
            using var context = await CreateContextAsync();
            var repo = new CommentRepository(context);
            var post = context.Posts.First();
            var user = context.Users.First();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "This is a test comment.",
                PostId = post.Id,
                AuthorId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            var added = await repo.AddAsync(comment);

            Assert.NotNull(added);
            Assert.Single(context.Comments);
            Assert.Equal(comment.Content, added.Content);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveComment()
        {
            using var context = await CreateContextAsync();
            var repo = new CommentRepository(context);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "To be deleted.",
                PostId = context.Posts.First().Id,
                AuthorId = context.Users.First().Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(comment.Id);

            Assert.Empty(context.Comments);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnComment()
        {
            using var context = await CreateContextAsync();
            var repo = new CommentRepository(context);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "To be retrieved.",
                PostId = context.Posts.First().Id,
                AuthorId = context.Users.First().Id,
                Author = context.Users.First(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(comment.Id);

            Assert.NotNull(result);
            Assert.Equal(comment.Content, result.Content);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateComment()
        {
            using var context = await CreateContextAsync();
            var repo = new CommentRepository(context);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Initial content.",
                PostId = context.Posts.First().Id,
                AuthorId = context.Users.First().Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            comment.Content = "Updated content.";
            var updated = await repo.UpdateAsync(comment);

            Assert.Equal("Updated content.", updated.Content);
        }

        [Fact]
        public async Task GetCommentsByPostAsync_ShouldReturnComments()
        {
            using var context = await CreateContextAsync();
            var repo = new CommentRepository(context);
            var post = context.Posts.First();
            var user = context.Users.First();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Test comment",
                PostId = post.Id,
                AuthorId = user.Id,
                Author = user,
                CreatedAt = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            var result = await repo.GetCommentsByPostAsync(post.Id);

            Assert.Single(result);
        }
    }
}
