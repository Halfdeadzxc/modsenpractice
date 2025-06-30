using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IInteractionService
    {
        Task AddBookmarkAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task RemoveBookmarkAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDTO>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CommentDTO> AddCommentAsync(CommentCreateDTO dto, Guid authorId, CancellationToken cancellationToken = default);
        Task DeleteCommentAsync(Guid commentId, Guid authorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(Guid postId, CancellationToken cancellationToken = default);
        Task AddLikeAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task RemoveLikeAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfileDTO>> GetLikesAsync(Guid postId, CancellationToken cancellationToken = default);
        Task AddRepostAsync(Guid userId, RepostCreateDTO dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDTO>> GetRepostsAsync(Guid postId, CancellationToken cancellationToken = default);
        Task SubscribeAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfileDTO>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfileDTO>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
