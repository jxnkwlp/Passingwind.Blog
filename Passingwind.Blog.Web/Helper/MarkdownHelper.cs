using CommonMark;
using Microsoft.AspNetCore.Html;

namespace Passingwind.Blog.Web
{
    public class MarkdownHelper
    {
        public static HtmlString CoventToHtml(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new HtmlString("");

            if (source.Contains("<p>"))
                return new HtmlString(source);

            return new HtmlString(CommonMarkConverter.Convert(source));
        }

    }
}
