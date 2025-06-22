using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public string Hashtags { get; set; }
        public int LikeCount { get; set; }
        public int RepostCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public User Author { get; set; }
        public int CommentCount { get; set; }
        public int BookmarkCount { get; set; }

    }
}
