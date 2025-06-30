using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace modsenpractice.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserProfileAsync(userId, cancellationToken);
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser(
            [FromBody] UserUpdateDTO dto,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _userService.UpdateUserProfileAsync(userId, dto, cancellationToken);
            return NoContent();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserProfileAsync(userId, cancellationToken);
            return Ok(user);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
