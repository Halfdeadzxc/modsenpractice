using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFeedRepository
    {
        Task<IEnumerable<Post>> GetPostsByAuthorsAsync(IEnumerable<Guid> authorIds, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetPopularPostsAsync(DateTime since, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }

}
