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

        public AuthService(
            IUserRepository userRepo,
            IConfiguration config,
            IEmailService emailService,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _config = config;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        public async Task<TokenResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (await _userRepo.GetByEmailAsync(dto.Email) != null)
                throw new ArgumentException("Email already exists");

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = _passwordService.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            return await GenerateTokens(user);
        }

        public async Task<TokenResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return await GenerateTokens(user);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new SecurityTokenException("Invalid refresh token");

            return await GenerateTokens(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null) return;

            user.PasswordResetToken = GenerateResetToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userRepo.UpdateAsync(user);

            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.Username,
                user.PasswordResetToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null ||
                user.PasswordResetToken != dto.Token ||
                user.ResetTokenExpires < DateTime.UtcNow)
                throw new ArgumentException("Invalid reset token");

            user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _userRepo.UpdateAsync(user);
        }

        private async Task<TokenResponseDTO> GenerateTokens(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepo.UpdateAsync(user);

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
