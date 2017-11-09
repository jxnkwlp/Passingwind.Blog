using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public class ArchiveViewModel
    {
        public IList<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public IList<PostViewModel> NoCategoryPosts { get; set; } = new List<PostViewModel>();

    }
}
