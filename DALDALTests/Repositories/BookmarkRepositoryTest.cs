using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DAL.Repositories.DALTests
{
    public class BookmarkRepositoryDALTest
    {
        private async Task<AppDbContext> GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Предварительная инициализация
            var post = new Post { Id = Guid.NewGuid(), Content = "Test Post" };
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

            context.Users.Add(user);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task AddAsync_AddsBookmarkSuccessfully()
        {
            using var context = await GetInMemoryDbContextAsync();
            var repo = new BookmarkRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;

            var bookmark = new Bookmark { UserId = userId, PostId = postId };
            var result = await repo.AddAsync(bookmark);

            Assert.NotNull(result);
            Assert.Single(context.Bookmarks);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBookmark()
        {
            using var context = await GetInMemoryDbContextAsync();
            var repo = new BookmarkRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;

            context.Bookmarks.Add(new Bookmark { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            await repo.DeleteAsync(userId, postId);
            Assert.Empty(context.Bookmarks);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBookmark()
        {
            using var context = await GetInMemoryDbContextAsync();
            var repo = new BookmarkRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;

            context.Bookmarks.Add(new Bookmark { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(userId, postId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueIfExists()
        {
            using var context = await GetInMemoryDbContextAsync();
            var repo = new BookmarkRepository(context);

            var userId = context.Users.First().Id;
            var postId = context.Posts.First().Id;

            context.Bookmarks.Add(new Bookmark { UserId = userId, PostId = postId });
            await context.SaveChangesAsync();

            var exists = await repo.ExistsAsync(userId, postId);
            Assert.True(exists);
        }
    }
}