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
    public class UserRepositoryTests
    {
        private async Task<AppDbContext> CreateContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new AppDbContext(options);
            return ctx;
        }

        private static User MakeUser()
            => new User
            {
                Id = Guid.NewGuid(),
                Email = "u@example.com",
                Username = "user1",
                PasswordHash = "pw-hash",
                AvatarUrl = "",
                Bio = "",
                RefreshToken = "ref-token",
                CreatedAt = DateTime.UtcNow
            };

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();

            var added = await repo.AddAsync(user);

            Assert.NotNull(added);
            Assert.Single(ctx.Users);
            Assert.Equal(user.Email, added.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            await repo.AddAsync(user);

            var found = await repo.GetByIdAsync(user.Id);

            Assert.NotNull(found);
            Assert.Equal(user.Username, found.Username);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            await repo.AddAsync(user);

            var found = await repo.GetByEmailAsync(user.Email);

            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            await repo.AddAsync(user);

            user.Bio = "Updated bio";
            var updated = await repo.UpdateAsync(user);

            Assert.Equal("Updated bio", updated.Bio);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldChangePasswordHash()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            await repo.AddAsync(user);

            var newHash = "new-hash";
            var result = await repo.UpdatePasswordAsync(user.Id, newHash);

            Assert.NotNull(result);
            Assert.Equal(newHash, result.PasswordHash);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            user.RefreshToken = "special-token";
            await repo.AddAsync(user);

            var found = await repo.GetByRefreshTokenAsync("special-token");

            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            await using var ctx = await CreateContextAsync();
            var repo = new UserRepository(ctx);
            var user = MakeUser();
            await repo.AddAsync(user);

            await repo.DeleteAsync(user.Id);

            Assert.Empty(ctx.Users);
        }
    }
}
