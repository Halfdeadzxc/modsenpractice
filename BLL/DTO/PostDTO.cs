using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
        public List<string> MediaUrls { get; set; }
        public string Hashtags { get; set; }
        public int LikeCount { get; set; }
        public int RepostCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsLiked { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsReposted { get; set; }

        public int CommentCount { get; set; }
        public int BookmarkCount { get; set; }
    }
}
