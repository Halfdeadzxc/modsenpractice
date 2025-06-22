using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPostService
    {
        Task<PostDTO> CreatePostAsync(Guid authorId, PostCreateDTO postDto);
        Task<PostDTO> UpdatePostAsync(Guid postId, Guid userId, PostCreateDTO postDto);
        Task DeletePostAsync(Guid postId, Guid userId);
        Task<PostDTO> GetPostByIdAsync(Guid postId);
        Task<IEnumerable<PostDTO>> GetPostsByUserAsync(Guid userId);
        Task<IEnumerable<PostDTO>> GetPostsByHashtagAsync(string hashtag);
    }
}
