using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> CreateAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task<bool> DeleteAsync(Guid postId);
        Task<Post> GetByIdAsync(Guid postId);
        Task<IEnumerable<Post>> GetByUserAsync(Guid userId);
        Task<IEnumerable<Post>> GetByHashtagAsync(string hashtag);
        Task IncrementLikeCountAsync(Guid postId);
        Task DecrementLikeCountAsync(Guid postId);
        Task IncrementCommentCountAsync(Guid postId);
        Task DecrementCommentCountAsync(Guid postId);
        Task IncrementRepostCountAsync(Guid postId);
        Task IncrementBookmarkCountAsync(Guid postId);
        Task DecrementBookmarkCountAsync(Guid postId);
    }
}
