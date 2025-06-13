using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepostRepository
    {
        Task<bool> AddRepostAsync(Guid userId, Guid postId, string comment);
        Task<IEnumerable<Post>> GetRepostsAsync(Guid postId);
    }
}
