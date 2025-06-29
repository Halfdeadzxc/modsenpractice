using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace modsenpractice.Controllers
{
    [Authorize]
    [Route("api/feed")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IFeedService _feedService;

        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string filter = "recent",
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var feed = await _feedService.GetFeedAsync(userId, page, pageSize, filter, cancellationToken);
            return Ok(feed);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
