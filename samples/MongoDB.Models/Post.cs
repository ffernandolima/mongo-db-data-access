using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Models
{
    public class Post
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public bool ShouldSerializeComments() => Comments?.Any() ?? false;
    }
}
