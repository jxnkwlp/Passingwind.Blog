using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web
{
    public static class StringExtensions
    {
        public static HtmlString ToHtmlString(this string source)
        {
            return MarkdownHelper.CoventToHtml(source);
        }
    }
}
