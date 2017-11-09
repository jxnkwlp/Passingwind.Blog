using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public class CategoryViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }


        public string Slug { get; set; }

        public int DisplayOrder { get; set; } = 1;


        public int Count { get; set; }


        public IList<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
