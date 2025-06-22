using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> AddLikeAsync(Guid userId, Guid postId);
        Task<bool> RemoveLikeAsync(Guid userId, Guid postId);
        Task<IEnumerable<User>> GetLikesAsync(Guid postId);
        Task<Like> GetLikeAsync(Guid userId, Guid postId);
        Task<bool> ExistsAsync(Guid userId, Guid postId);
    }
}
