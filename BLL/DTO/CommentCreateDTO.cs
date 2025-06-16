using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class CommentCreateDTO
    {
       public Guid PostId{get; set;}
      public string Content { get; set; }
        public Guid? ParentCommentId { get; set; } = null;
}
}
