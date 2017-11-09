using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Passingwind.Blog.Pager;

namespace Passingwind.Blog.Web.Pager
{
    public static class HtmlExtensions
    {
        public static MvcPager Pager(this IHtmlHelper htmlHelper, IPagedList pagedList)
        {
            return new MvcPager(htmlHelper, pagedList, new UrlHelperFactory());
        }
    }
}
