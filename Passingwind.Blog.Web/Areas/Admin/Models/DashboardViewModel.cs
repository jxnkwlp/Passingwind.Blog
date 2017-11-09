using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalPostsCount { get; set; }
        public int TotalPagesCount { get; set; }
        public int TotalCategoriesCount { get; set; }
        public int TotalTagsCount { get; set; }
        public int TotalCommentsCount { get; set; }

    }
}
