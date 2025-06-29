using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;    
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using DAL;

namespace Tests.DALTests
{
    using DAL.Models;
    using DAL.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using System.Linq;
    using DAL;

    namespace Tests.DALTests
    {
        public class BookmarkRepositoryTests
        {
            private async Task<AppDbContext> GetInMemoryDbContextAsync()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                var context = new AppDbContext(options);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "test@example.com",
                    Username = "TestUser",
                    PasswordHash = "hashed-password",
                    AvatarUrl = "",
                    Bio = "",
                    RefreshToken = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Content = "Test Post",
                    AuthorId = user.Id,
                    Hashtags = "#test",
                    MediaUrls = new List<string>(),
                    Author = user
                };

                await context.Users.AddAsync(user);
                await context.Posts.AddAsync(post);
                await context.SaveChangesAsync();

                return context;
            }


            [Fact]
            public async Task AddAsync_ShouldAddBookmark()
            {
                using var context = await GetInMemoryDbContextAsync();
                var repo = new BookmarkRepository(context);

                var userId = context.Users.First().Id;
                var postId = context.Posts.First().Id;

                var bookmark = new Bookmark { UserId = userId, PostId = postId };
                var result = await repo.AddAsync(bookmark);

                Assert.NotNull(result);
                Assert.Single(context.Bookmarks);
                Assert.Equal(userId, result.UserId);
            }

            [Fact]
            public async Task DeleteAsync_ShouldRemoveBookmark()
            {
                using var context = await GetInMemoryDbContextAsync();
                var repo = new BookmarkRepository(context);

                var userId = context.Users.First().Id;
                var postId = context.Posts.First().Id;

                await context.Bookmarks.AddAsync(new Bookmark { UserId = userId, PostId = postId });
                await context.SaveChangesAsync();

                await repo.DeleteAsync(userId, postId);

                var remaining = await context.Bookmarks.ToListAsync();
                Assert.Empty(remaining);
            }

            [Fact]
            public async Task GetByIdAsync_ShouldReturnBookmark()
            {
                using var context = await GetInMemoryDbContextAsync();
                var repo = new BookmarkRepository(context);

                var userId = context.Users.First().Id;
                var postId = context.Posts.First().Id;

                await context.Bookmarks.AddAsync(new Bookmark { UserId = userId, PostId = postId });
                await context.SaveChangesAsync();

                var result = await repo.GetByIdAsync(userId, postId);

                Assert.NotNull(result);
                Assert.Equal(userId, result.UserId);
                Assert.Equal(postId, result.PostId);
            }

            [Fact]
            public async Task ExistsAsync_ShouldReturnTrueIfExists()
            {
                using var context = await GetInMemoryDbContextAsync();
                var repo = new BookmarkRepository(context);

                var userId = context.Users.First().Id;
                var postId = context.Posts.First().Id;

                await context.Bookmarks.AddAsync(new Bookmark { UserId = userId, PostId = postId });
                await context.SaveChangesAsync();

                var exists = await repo.ExistsAsync(userId, postId);

                Assert.True(exists);
            }
        }
    }

}
