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
        Task<IEnumerable<Post>> GetFeedAsync(Guid userId, int page, int pageSize, string filter);
    }
}
