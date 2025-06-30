using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class RepostCreateDTO
    {
        public Guid PostId { get; set; }
        public string Comment { get; set; }
    }
}
