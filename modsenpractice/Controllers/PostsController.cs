using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace modsenpractice.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(
            [FromBody] PostCreateDTO dto,
            CancellationToken cancellationToken = default)
        {
            var authorId = GetCurrentUserId();
            var post = await _postService.CreatePostAsync(authorId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(
            Guid id,
            [FromBody] PostCreateDTO dto,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var post = await _postService.UpdatePostAsync(id, userId, dto, cancellationToken);
            return Ok(post);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _postService.DeletePostAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var post = await _postService.GetPostByIdAsync(id, cancellationToken);
            return Ok(post);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var posts = await _postService.GetPostsByUserAsync(userId, cancellationToken);
            return Ok(posts);
        }

        [HttpGet("hashtag/{hashtag}")]
        public async Task<IActionResult> GetPostsByHashtag(
            string hashtag,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var posts = await _postService.GetPostsByHashtagAsync(hashtag, cancellationToken);
            return Ok(posts);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
