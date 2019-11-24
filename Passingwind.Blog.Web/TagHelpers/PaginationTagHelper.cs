using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Passingwind.Blog.Pager;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement("pager-info")]
    public class PagerInfoTagHelpr : TagHelper
    {
        [HtmlAttributeName("data")]
        public IPagedList Data { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            output.TagName = "div";

            if (Data.TotalItems == 0)
            {
                output.Content.SetContent($"共{Data.TotalItems}项");
            }
            else
            {
                output.Content.SetContent($"第{Data.PageNumber}/{Data.TotalPages}页，共{Data.TotalItems}项");
            }
        }
    }

    [HtmlTargetElement("pager")]
    public class PaginationTagHelper : TagHelper
    {
        [HtmlAttributeName("data")]
        public IPagedList Data { get; set; }

        public int ItemCount { get; set; } = 5;

        public string QueryName { get; set; } = "page";



        public override int Order => 1;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        protected IUrlHelperFactory UrlHelperFactory;

        public PaginationTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            UrlHelperFactory = urlHelperFactory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ItemCount <= 1)
                ItemCount = 1;

            if (Data != null)
            {
                var html = WritePagerBar();

                output.Content.SetHtmlContent(html);
            }

            if (output.Attributes.ContainsName("class"))
            {
                var old = output.Attributes["class"];
                output.Attributes.Remove(old);

                output.Attributes.Add("class", "pagerbar pagination " + old.Value);
            }
            else
            {
                output.Attributes.Add("class", "pagerbar pagination ");
            }
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "ul";
        }



        private IHtmlContent WritePagerBar()
        {
            HtmlContentBuilder builder = new HtmlContentBuilder();

            //builder.AppendHtml(WriteItem("首页", 1, HasFirst(Data), false));
            builder.AppendHtml(WriteItem("上一页", Data.PageNumber - 1, HasPrevious(Data), false));

            if (HasPreviousMore(Data, ItemCount))
            {
                builder.AppendHtml(WriteItem("...", Data.PageNumber - 1, false, false));
            }

            var start = GetStartNumber(Data);
            var end = GetEndNumber(Data);

            for (int i = start; i <= end; i++)
            {
                builder.AppendHtml(WriteItem(i.ToString(), i, Data.PageNumber != i, Data.PageNumber == i));
            }

            if (HasNextMore(Data, ItemCount))
            {
                builder.AppendHtml(WriteItem("...", Data.PageNumber + 1, false, false));
            }

            builder.AppendHtml(WriteItem("下一页", Data.PageNumber + 1, HasNext(Data), false));
            // builder.AppendHtml(WriteItem("末页", Data.TotalPages, HasLast(Data), false));

            return builder;
        }



        private TagBuilder WriteItem(string text, int page, bool hasLink, bool active)
        {
            TagBuilder tagBuilder = new TagBuilder("li");

            tagBuilder.AddCssClass("pager-item");

            if (hasLink)
            {
                var link = new TagBuilder("a");
                link.Attributes["href"] = BuilderLink(page);
                link.InnerHtml.SetContent(text);
                tagBuilder.InnerHtml.AppendHtml(link);
            }
            else
            {
                var link = new TagBuilder("span");
                link.InnerHtml.SetContent(text);
                tagBuilder.InnerHtml.AppendHtml(link);
            }

            if (active)
            {
                tagBuilder.AddCssClass("active");
            }

            return tagBuilder;
        }


        RouteValueDictionary GetQueryStrings(int pageNumber)
        {
            var routeValues = new RouteValueDictionary(); //_html.ViewContext.RouteData.Values;
            var queryStrings = ViewContext.HttpContext.Request.Query;

            foreach (var key in queryStrings.Keys.Where(key => key != null))
            {
                var value = queryStrings[key];
                routeValues[key] = value;
            }

            if (pageNumber == 1 && Data.PageNumber == 1)
            {
                if (routeValues.ContainsKey(QueryName))
                    routeValues.Remove(QueryName);
            }
            else
            {
                routeValues[QueryName] = pageNumber;
            }

            return routeValues;
        }

        string BuilderLink(int page)
        {
            var query = GetQueryStrings(page);

            var urlHelper = UrlHelperFactory.GetUrlHelper(ViewContext);

            var queryString = string.Join("&", query.Select(t => t.Key + "=" + t.Value));

            return ViewContext.HttpContext.Request.Path + "?" + queryString;
        }


        static bool HasFirst(IPagedList pagedList)
        {
            return pagedList.PageNumber > 1;
        }

        static bool HasPrevious(IPagedList pagedList)
        {
            return pagedList.PageNumber > 1;
        }

        static bool HasNext(IPagedList pagedList)
        {
            return pagedList.PageNumber < pagedList.TotalPages;
        }

        static bool HasLast(IPagedList pagedList)
        {
            return pagedList.PageNumber < pagedList.TotalPages;
        }

        static int GetPreviousNumber(IPagedList pagedList)
        {
            var i = pagedList.PageNumber - 1;

            return i <= 1 ? 1 : i;
        }

        static int GetNextNumber(IPagedList pagedList)
        {
            var i = pagedList.PageNumber + 1;

            return i >= pagedList.TotalPages ? pagedList.TotalPages : i;
        }


        static int GetStartNumber(IPagedList pagedList, int itemCount = 5)
        {
            var s = itemCount / 2;

            var index = pagedList.PageNumber - s;

            if (pagedList.TotalPages - itemCount < index)
                index = pagedList.TotalPages - itemCount + 1;

            if (index <= 1)
                index = 1;

            return index;
        }

        static int GetEndNumber(IPagedList pagedList, int itemCount = 5)
        {
            var s = itemCount / 2;

            var index = pagedList.PageNumber + s;

            if (index <= itemCount)
                index = itemCount;

            if (index >= pagedList.TotalPages)
                index = pagedList.TotalPages;

            return index;
        }


        static bool HasNextMore(IPagedList pagedList, int itemCount = 5)
        {
            var end = GetEndNumber(pagedList, itemCount);

            return (end < pagedList.TotalPages);
        }

        static bool HasPreviousMore(IPagedList pagedList, int itemCount = 5)
        {
            var start = GetStartNumber(pagedList, itemCount);

            return (start > 1);
        }

    }
}
