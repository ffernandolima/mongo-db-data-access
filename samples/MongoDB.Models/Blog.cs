using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public BlogType Type { get; set; }
        public IList<Post> Posts { get; set; } = new List<Post>();
        public bool ShouldSerializePosts() => Posts?.Any() ?? false;
    }
}
