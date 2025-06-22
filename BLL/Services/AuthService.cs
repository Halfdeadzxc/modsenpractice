using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepo,
            IConfiguration config,
            IEmailService emailService,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _userRepo = userRepo;
            _config = config;
            _emailService = emailService;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<TokenResponseDTO> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (await _userRepo.GetByEmailAsync(dto.Email, cancellationToken) != null)
                throw new ArgumentException("Email already exists");

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = _passwordService.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user, cancellationToken);
            return await GenerateTokens(user, cancellationToken);
        }

        public async Task<TokenResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return await GenerateTokens(user, cancellationToken);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new SecurityTokenException("Invalid refresh token");

            return await GenerateTokens(user, cancellationToken);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null) return;

            user.PasswordResetToken = _jwtService.GenerateResetToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userRepo.UpdateAsync(user, cancellationToken);

            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.Username,
                user.PasswordResetToken,
                cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null ||
                user.PasswordResetToken != dto.Token ||
                user.ResetTokenExpires < DateTime.UtcNow)
                throw new ArgumentException("Invalid reset token");

            user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _userRepo.UpdateAsync(user, cancellationToken);
        }

        private async Task<TokenResponseDTO> GenerateTokens(User user, CancellationToken cancellationToken = default)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepo.UpdateAsync(user, cancellationToken);

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

    }
}
