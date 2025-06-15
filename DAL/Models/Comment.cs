using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
