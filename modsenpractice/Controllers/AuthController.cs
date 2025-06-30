using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace modsenpractice.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDTO dto,
            CancellationToken cancellationToken = default)
        {
            var tokens = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO dto,
            CancellationToken cancellationToken = default)
        {
            var tokens = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] TokenResponseDTO dto,
            CancellationToken cancellationToken = default)
        {
            var tokens = await _authService.RefreshTokenAsync(dto.RefreshToken, cancellationToken);
            return Ok(tokens);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDTO dto,
            CancellationToken cancellationToken = default)
        {
            await _authService.ForgotPasswordAsync(dto, cancellationToken);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDTO dto,
            CancellationToken cancellationToken = default)
        {
            await _authService.ResetPasswordAsync(dto, cancellationToken);
            return Ok();
        }
    }
}
