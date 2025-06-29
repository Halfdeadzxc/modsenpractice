using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.BLLTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IJwtService> _jwt = new();
        private readonly Mock<IEmailService> _email = new();
        private readonly Mock<IPasswordService> _password = new();
        private readonly Mock<IConfiguration> _config = new();
        private readonly AuthService _auth;
        private readonly CancellationToken _ct = default;

        public AuthServiceTests()
        {
            _auth = new AuthService(_userRepo.Object, _config.Object, _email.Object, _password.Object, _jwt.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowIfEmailExists()
        {
            _userRepo.Setup(r => r.GetByEmailAsync("x@x.com", _ct)).ReturnsAsync(new User());
            var dto = new RegisterDTO { Email = "x@x.com", Username = "u", Password = "p" };

            await Assert.ThrowsAsync<ArgumentException>(() => _auth.RegisterAsync(dto, _ct));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTokens()
        {
            var dto = new RegisterDTO { Email = "x@x.com", Username = "u", Password = "p" };

            _userRepo
                .Setup(r => r.GetByEmailAsync(dto.Email, _ct))
                .ReturnsAsync((User)null);

            _password
                .Setup(p => p.HashPassword("p"))
                .Returns("hash");

            _userRepo
                .Setup(r => r.AddAsync(It.IsAny<User>(), _ct))
                .ReturnsAsync((User u, CancellationToken _) => u);

            _userRepo
                .Setup(r => r.UpdateAsync(It.IsAny<User>(), _ct))
                .ReturnsAsync((User u, CancellationToken _) => u);

            _jwt
                .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
                .Returns("access");

            _jwt
                .Setup(j => j.GenerateRefreshToken())
                .Returns("refresh");

            var res = await _auth.RegisterAsync(dto, _ct);

            Assert.Equal("access", res.AccessToken);
            Assert.Equal("refresh", res.RefreshToken);
        }


        [Fact]
        public async Task LoginAsync_ShouldThrowIfInvalid()
        {
            var dto = new LoginDTO { Email = "x@x.com", Password = "p" };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.Email, _ct)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _auth.LoginAsync(dto, _ct));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens()
        {
            var dto = new LoginDTO { Email = "e", Password = "p" };
            var u = new User { Email = "e", PasswordHash = "hash" };
            _userRepo.Setup(r => r.GetByEmailAsync("e", _ct)).ReturnsAsync(u);
            _password.Setup(p => p.VerifyPassword("p", "hash")).Returns(true);
            _jwt.Setup(j => j.GenerateAccessToken(u)).Returns("access");
            _jwt.Setup(j => j.GenerateRefreshToken()).Returns("refresh");
            _userRepo.Setup(r => r.UpdateAsync(u, _ct)).ReturnsAsync(u);

            var res = await _auth.LoginAsync(dto, _ct);

            Assert.Equal("access", res.AccessToken);
            Assert.Equal("refresh", res.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowIfInvalid()
        {
            _userRepo.Setup(r => r.GetByRefreshTokenAsync("bad", _ct)).ReturnsAsync((User)null);
            await Assert.ThrowsAsync<SecurityTokenException>(() => _auth.RefreshTokenAsync("bad", _ct));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnTokens()
        {
            var user = new User { RefreshToken = "r", RefreshTokenExpiry = DateTime.UtcNow.AddDays(1) };
            _userRepo.Setup(r => r.GetByRefreshTokenAsync("r", _ct)).ReturnsAsync(user);
            _jwt.Setup(j => j.GenerateAccessToken(user)).Returns("access");
            _jwt.Setup(j => j.GenerateRefreshToken()).Returns("refresh");
            _userRepo.Setup(r => r.UpdateAsync(user, _ct)).ReturnsAsync(user);

            var res = await _auth.RefreshTokenAsync("r", _ct);

            Assert.Equal("access", res.AccessToken);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldDoNothingIfUserNotFound()
        {
            _userRepo.Setup(r => r.GetByEmailAsync("x", _ct)).ReturnsAsync((User)null);
            await _auth.ForgotPasswordAsync(new ForgotPasswordDTO { Email = "x" }, _ct);
            _email.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _ct), Times.Never);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldSendToken()
        {
            var user = new User { Email = "e", Username = "u" };
            _userRepo.Setup(r => r.GetByEmailAsync("e", _ct)).ReturnsAsync(user);
            _jwt.Setup(j => j.GenerateResetToken()).Returns("token");
            _userRepo.Setup(r => r.UpdateAsync(user, _ct)).ReturnsAsync(user);

            await _auth.ForgotPasswordAsync(new ForgotPasswordDTO { Email = "e" }, _ct);

            Assert.Equal("token", user.PasswordResetToken);
            _email.Verify(e => e.SendPasswordResetEmailAsync("e", "u", "token", _ct), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldThrowIfInvalid()
        {
            _userRepo.Setup(r => r.GetByEmailAsync("e", _ct)).ReturnsAsync((User)null);
            await Assert.ThrowsAsync<ArgumentException>(() => _auth.ResetPasswordAsync(new ResetPasswordDTO
            {
                Email = "e",
                Token = "t",
                NewPassword = "p"
            }, _ct));
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldSucceed()
        {
            var u = new User { Email = "e", PasswordResetToken = "t", ResetTokenExpires = DateTime.UtcNow.AddMinutes(5) };
            _userRepo.Setup(r => r.GetByEmailAsync("e", _ct)).ReturnsAsync(u);
            _password.Setup(p => p.HashPassword("npw")).Returns("hashed");
            _userRepo.Setup(r => r.UpdateAsync(u, _ct)).ReturnsAsync(u);

            await _auth.ResetPasswordAsync(new ResetPasswordDTO
            {
                Email = "e",
                Token = "t",
                NewPassword = "npw"
            }, _ct);

            Assert.Equal("hashed", u.PasswordHash);
            Assert.Null(u.PasswordResetToken);
            Assert.Null(u.ResetTokenExpires);
        }
    }
}
