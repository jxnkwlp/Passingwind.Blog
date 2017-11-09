using Microsoft.AspNetCore.Razor.TagHelpers;
using Passingwind.Blog.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-pager")]
        public IPagedList PagedList { get; set; }

    }
}
