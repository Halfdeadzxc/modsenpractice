using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetByHashtagAsync(string hashtag, CancellationToken cancellationToken = default);
        
    }
}
