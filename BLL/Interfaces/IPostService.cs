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
        Task<PostDTO> CreatePostAsync(Guid authorId, PostCreateDTO dto, CancellationToken cancellationToken = default);
        Task<PostDTO> UpdatePostAsync(Guid postId, Guid userId, PostCreateDTO dto, CancellationToken cancellationToken = default);
        Task DeletePostAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);
        Task<PostDTO> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDTO>> GetPostsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDTO>> GetPostsByHashtagAsync(string hashtag, CancellationToken cancellationToken = default);
    }
}
