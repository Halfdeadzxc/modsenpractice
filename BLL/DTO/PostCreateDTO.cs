using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class PostCreateDTO
    {
        public string Content { get; set; }
        public List<string> MediaUrls { get; set; } = new();
        public string Hashtags { get; set; }
    }
}
