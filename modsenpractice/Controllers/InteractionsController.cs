using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace modsenpractice.Controllers
{
    [Authorize]
    [Route("api/interactions")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionsController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        #region Bookmarks
        [HttpPost("bookmarks/{postId}")]
        public async Task<IActionResult> AddBookmark(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _interactionService.AddBookmarkAsync(userId, postId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("bookmarks/{postId}")]
        public async Task<IActionResult> RemoveBookmark(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _interactionService.RemoveBookmarkAsync(userId, postId, cancellationToken);
            return NoContent();
        }

        [HttpGet("bookmarks")]
        public async Task<IActionResult> GetBookmarks(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var bookmarks = await _interactionService.GetBookmarksAsync(userId, cancellationToken);
            return Ok(bookmarks);
        }
        #endregion

        #region Comments
        [HttpPost("comments")]
        public async Task<IActionResult> AddComment(
            [FromBody] CommentCreateDTO dto,
            CancellationToken cancellationToken = default)
        {
            var authorId = GetCurrentUserId();
            var comment = await _interactionService.AddCommentAsync(dto, authorId, cancellationToken);
            return CreatedAtAction(nameof(GetCommentsByPost), new { id = comment.Id }, comment);
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var authorId = GetCurrentUserId();
            await _interactionService.DeleteCommentAsync(id, authorId, cancellationToken);
            return NoContent();
        }

        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetCommentsByPost(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var comments = await _interactionService.GetCommentsByPostAsync(postId, cancellationToken);
            return Ok(comments);
        }
        #endregion

        #region Likes
        [HttpPost("likes/{postId}")]
        public async Task<IActionResult> AddLike(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _interactionService.AddLikeAsync(userId, postId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("likes/{postId}")]
        public async Task<IActionResult> RemoveLike(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _interactionService.RemoveLikeAsync(userId, postId, cancellationToken);
            return NoContent();
        }

        [HttpGet("posts/{postId}/likes")]
        public async Task<IActionResult> GetLikes(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var likes = await _interactionService.GetLikesAsync(postId, cancellationToken);
            return Ok(likes);
        }
        #endregion

        #region Reposts
        [HttpPost("reposts")]
        public async Task<IActionResult> AddRepost(
            [FromBody] RepostCreateDTO dto,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _interactionService.AddRepostAsync(userId, dto, cancellationToken);
            return NoContent();
        }

        [HttpGet("posts/{postId}/reposts")]
        public async Task<IActionResult> GetReposts(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var reposts = await _interactionService.GetRepostsAsync(postId, cancellationToken);
            return Ok(reposts);
        }
        #endregion

        #region Subscriptions
        [HttpPost("subscriptions/{followingId}")]
        public async Task<IActionResult> Subscribe(
            Guid followingId,
            CancellationToken cancellationToken = default)
        {
            var followerId = GetCurrentUserId();
            await _interactionService.SubscribeAsync(followerId, followingId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("subscriptions/{followingId}")]
        public async Task<IActionResult> Unsubscribe(
            Guid followingId,
            CancellationToken cancellationToken = default)
        {
            var followerId = GetCurrentUserId();
            await _interactionService.UnsubscribeAsync(followerId, followingId, cancellationToken);
            return NoContent();
        }

        [HttpGet("users/{userId}/followers")]
        public async Task<IActionResult> GetFollowers(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var followers = await _interactionService.GetFollowersAsync(userId, cancellationToken);
            return Ok(followers);
        }

        [HttpGet("users/{userId}/following")]
        public async Task<IActionResult> GetFollowing(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var following = await _interactionService.GetFollowingAsync(userId, cancellationToken);
            return Ok(following);
        }
        #endregion

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
