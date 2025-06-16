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
        Task AddBookmarkAsync(Guid userId, Guid postId);
        Task RemoveBookmarkAsync(Guid userId, Guid postId);
        Task<IEnumerable<PostDTO>> GetBookmarksAsync(Guid userId);
        Task<CommentDTO> AddCommentAsync(CommentCreateDTO commentDto, Guid authorId);
        Task DeleteCommentAsync(Guid commentId, Guid authorId);
        Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(Guid postId);
        Task AddLikeAsync(Guid userId, Guid postId);
        Task RemoveLikeAsync(Guid userId, Guid postId);
        Task<IEnumerable<UserProfileDTO>> GetLikesAsync(Guid postId);
        Task AddRepostAsync(Guid userId, RepostCreateDTO repostDto);
        Task<IEnumerable<PostDTO>> GetRepostsAsync(Guid postId);
        Task SubscribeAsync(Guid followerId, Guid followingId);
        Task UnsubscribeAsync(Guid followerId, Guid followingId);
        Task<IEnumerable<UserProfileDTO>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<UserProfileDTO>> GetFollowingAsync(Guid userId);
    }
}
