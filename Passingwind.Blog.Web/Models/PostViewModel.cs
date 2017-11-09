using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public class PostViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }

        public bool IsDraft { get; set; }

        public int ViewsCount { get; set; }

        public int CommentsCount { get; set; }

        public bool EnableComment { get; set; }

        public DateTime PublishedTime { get; set; } = DateTime.Now;

        public UserViewModel User { get; set; }

        public IList<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public IList<string> Tags { get; set; } = new List<string>();

    }
}
