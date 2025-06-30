using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentDTO> Replies { get; set; } = new();

        public CommentDTO Parent { get; set; }
    }
}
