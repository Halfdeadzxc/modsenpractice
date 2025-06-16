using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFeedService
    {
        Task<IEnumerable<PostDTO>> GetFeedAsync(
            Guid userId,
            int page = 1,
            int pageSize = 10,
            string filter = "recent");
    }
}
