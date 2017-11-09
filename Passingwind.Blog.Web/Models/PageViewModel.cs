using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public class PageViewModel
    {

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

        public bool IsFrontPage { get; set; }

        public bool IsShowInList { get; set; }

        public bool Published { get; set; } 

        public int DisplayOrder { get; set; }


    }
}
