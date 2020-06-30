using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Text.Encodings.Web;

namespace Passingwind.Blog.Web.TagHelpers
{
	//[HtmlTargetElement("pager-info")]
	//public class PagerInfoTagHelpr : TagHelper
	//{
	//	[HtmlAttributeName("data")]
	//	public IPagedList Data { get; set; }

	//	public override void Process(TagHelperContext context, TagHelperOutput output)
	//	{
	//		base.Process(context, output);

	//		output.TagName = "div";

	//		if (Data.TotalCount == 0)
	//		{
	//			output.Content.SetContent($"共{Data.TotalCount}项");
	//		}
	//		else
	//		{
	//			output.Content.SetContent($"第{Data.PageNumber}/{Data.TotalPage}页，共{Data.TotalCount}项");
	//		}
	//	}
	//}


	public enum PaginationItemShow
	{
		Auto = 0,
		Always,
		Never
	}

	[HtmlTargetElement("pager")]
	public class PaginationTagHelper : TagHelper
	{
		public int CurrentPage { get; set; }
		public int Total { get; set; }
		public int PageSize { get; set; }

		public bool ShowTitle { get; set; }

		public int PageItemsCount { get; set; } = 5;
		public string QueryName { get; set; } = "page";

		public PaginationItemShow PreviouPageShow { get; set; }
		public PaginationItemShow NextPageShow { get; set; }
		public PaginationItemShow ItemsShow { get; set; }
		public PaginationItemShow FirstPageShow { get; set; }
		public PaginationItemShow LastPageShow { get; set; }

		public Func<IHtmlContent, bool, TagBuilder> ItemWarpBuild { get; set; }
		public Func<string, bool, string> TextBuild { get; set; }

		public string PaginationClass { get; set; }
		public string PageItemClass { get; set; }
		public string PageItemLinkClass { get; set; }
		public string ActiveItemClass { get; set; } = "active";

		public string TagName { get; set; } = "div";

		public override int Order => 1;

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		private PagedList _pagedList;


		protected IUrlHelperFactory UrlHelperFactory;

		public PaginationTagHelper(IUrlHelperFactory urlHelperFactory)
		{
			UrlHelperFactory = urlHelperFactory;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			base.Process(context, output);

			if (PageItemsCount <= 1) PageItemsCount = 1;
			if (CurrentPage <= 1) CurrentPage = 1;

			_pagedList = new PagedList()
			{
				PageNumber = CurrentPage,
				PageSize = PageSize,
				TotalCount = Total,
				TotalPage = GetTotalPageCount(),
			};


			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = TagName;
			output.AddClass(PaginationClass, HtmlEncoder.Default);


			var html = Build();

			output.Content.SetHtmlContent(html);

			//if (Data != null)
			//{
			//	var html = WritePagerBar();

			//	output.Content.SetHtmlContent(html);
			//}

			//if (output.Attributes.ContainsName("class"))
			//{
			//	var old = output.Attributes["class"];
			//	output.Attributes.Remove(old);

			//	output.Attributes.Add("class", "pagerbar pagination " + old.Value);
			//}
			//else
			//{
			//	output.Attributes.Add("class", "pagerbar pagination ");
			//}
			//output.TagMode = TagMode.StartTagAndEndTag;
			//output.TagName = "ul";
		}



		private IHtmlContent Build()
		{
			HtmlContentBuilder builder = new HtmlContentBuilder();

			var currentPage = _pagedList.PageNumber;

			// first page 
			if (FirstPageShow == PaginationItemShow.Always || (FirstPageShow == PaginationItemShow.Auto && HasFirst(_pagedList)))
			{
				var page = 1;
				builder.AppendHtml(BuildPageItem(page, "First", page != currentPage, page == currentPage));
			}

			// previous page
			if (PreviouPageShow == PaginationItemShow.Always || (PreviouPageShow == PaginationItemShow.Auto && HasPrevious(_pagedList)))
			{
				var page = GetPreviousNumber(_pagedList);
				builder.AppendHtml(BuildPageItem(page, "Previous", page != currentPage, page == currentPage));
			}

			if (HasPreviousMore(_pagedList))
			{
				builder.AppendHtml(BuildPageItem(0, "...", false, false));
			}

			// 中间页码 
			var start = GetStartNumber(_pagedList);
			var end = GetEndNumber(_pagedList);

			for (int i = start; i <= end; i++)
			{
				var item = BuildPageItem(i, i.ToString(), i != currentPage, i == currentPage);
				builder.AppendHtml(item);
			}

			if (HasNextMore(_pagedList))
			{
				builder.AppendHtml(BuildPageItem(0, "...", false, false));
			}

			// next page 
			if (NextPageShow == PaginationItemShow.Always || (NextPageShow == PaginationItemShow.Auto && HasNext(_pagedList)))
			{
				var page = GetNextNumber(_pagedList);
				builder.AppendHtml(BuildPageItem(page, "Next", page != currentPage, page == currentPage));
			}

			// last page
			if (LastPageShow == PaginationItemShow.Always || (LastPageShow == PaginationItemShow.Auto && HasLast(_pagedList)))
			{
				var page = _pagedList.TotalPage;
				builder.AppendHtml(BuildPageItem(page, "Last", page != currentPage, page == currentPage));
			}

			return builder;
		}

		private IHtmlContent BuildPageItem(int page, string text, bool link, bool active)
		{
			TagBuilder tagBuilder = new TagBuilder(link ? "a" : "span");

			if (TextBuild != null) text = TextBuild(text, active);

			tagBuilder.InnerHtml.SetContent(text);

			if (active && TextBuild == null)
				tagBuilder.AddCssClass(ActiveItemClass);

			tagBuilder.AddCssClass(PageItemClass ?? "");
			if (link)
				tagBuilder.AddCssClass(PageItemLinkClass ?? "");

			if (ShowTitle)
				tagBuilder.Attributes["title"] = text;

			if (link)
			{
				tagBuilder.Attributes["href"] = BuilderLink(page);
			}

			if (ItemWarpBuild != null)
			{
				var tag2 = ItemWarpBuild(tagBuilder, active);
				if (active) tag2.AddCssClass(ActiveItemClass);

				return tag2;
			}

			return tagBuilder;
		}



		//private IHtmlContent WritePagerBar()
		//{
		//	HtmlContentBuilder builder = new HtmlContentBuilder();

		//	if (ShowFirstPage)
		//		builder.AppendHtml(WriteItem("上一页", Data.PageNumber - 1, HasPrevious(Data), false));

		//	if (HasPreviousMore(Data, ItemCount))
		//	{
		//		builder.AppendHtml(WriteItem("...", Data.PageNumber - 1, false, false));
		//	}

		//	var start = GetStartNumber(Data);
		//	var end = GetEndNumber(Data);

		//	if (start != end)
		//	{
		//		for (int i = start; i <= end; i++)
		//		{
		//			builder.AppendHtml(WriteItem(i.ToString(), i, Data.PageNumber != i, Data.PageNumber == i));
		//		}
		//	}

		//	if (HasNextMore(Data, ItemCount))
		//	{
		//		builder.AppendHtml(WriteItem("...", Data.PageNumber + 1, false, false));
		//	}

		//	if (ShowLastPage)
		//		builder.AppendHtml(WriteItem("下一页", Data.PageNumber + 1, HasNext(Data), false));

		//	return builder;
		//}



		//private TagBuilder WriteItem(string text, int page, bool hasLink, bool active)
		//{
		//	TagBuilder tagBuilder = new TagBuilder("li");

		//	tagBuilder.AddCssClass($"page-item {PageItemClass}");

		//	if (hasLink)
		//	{
		//		var link = new TagBuilder("a");
		//		link.Attributes["href"] = BuilderLink(page);
		//		link.InnerHtml.SetContent(text);
		//		link.AddCssClass($"page-link {PageItemLinkClass}");
		//		tagBuilder.InnerHtml.AppendHtml(link);
		//	}
		//	else
		//	{
		//		var link = new TagBuilder("span");
		//		link.InnerHtml.SetContent(text);
		//		link.AddCssClass($"page-link {PageItemLinkClass}");
		//		tagBuilder.InnerHtml.AppendHtml(link);
		//	}

		//	if (active)
		//	{
		//		tagBuilder.AddCssClass("active");
		//	}

		//	return tagBuilder;
		//}

		private RouteValueDictionary GetQueryStrings(int page)
		{
			var routeValues = new RouteValueDictionary(); //_html.ViewContext.RouteData.Values;
			var queryStrings = ViewContext.HttpContext.Request.Query;

			foreach (var key in queryStrings.Keys.Where(key => key != null))
			{
				var value = queryStrings[key];
				routeValues[key] = value;
			}

			if (page == 1)
			{
				if (routeValues.ContainsKey(QueryName))
					routeValues.Remove(QueryName);
			}
			else
			{
				routeValues[QueryName] = page;
			}

			return routeValues;
		}

		private string BuilderLink(int page)
		{
			var query = GetQueryStrings(page);

			var urlHelper = UrlHelperFactory.GetUrlHelper(ViewContext);

			var queryString = string.Join("&", query.Select(t => t.Key + "=" + t.Value));

			return ViewContext.HttpContext.Request.Path + "?" + queryString;
		}


		#region Utils

		private static bool HasFirst(PagedList pagedList)
		{
			return pagedList.PageNumber > 1;
		}

		private static bool HasPrevious(PagedList pagedList)
		{
			return pagedList.PageNumber > 1;
		}

		private static bool HasNext(PagedList pagedList)
		{
			return pagedList.PageNumber < pagedList.TotalPage;
		}

		private static bool HasLast(PagedList pagedList)
		{
			return pagedList.PageNumber < pagedList.TotalPage;
		}

		private static int GetPreviousNumber(PagedList pagedList)
		{
			var i = pagedList.PageNumber - 1;

			return i <= 1 ? 1 : i;
		}

		private static int GetNextNumber(PagedList pagedList)
		{
			var i = pagedList.PageNumber + 1;

			return i >= pagedList.TotalPage ? pagedList.TotalPage : i;
		}

		private static int GetStartNumber(PagedList pagedList, int itemCount = 5)
		{
			var s = itemCount / 2;

			var index = pagedList.PageNumber - s;

			if (pagedList.TotalPage - itemCount < index)
				index = pagedList.TotalPage - itemCount + 1;

			if (index <= 1)
				index = 1;

			return index;
		}

		private static int GetEndNumber(PagedList pagedList, int itemCount = 5)
		{
			var s = itemCount / 2;

			var index = pagedList.PageNumber + s;

			if (index <= itemCount)
				index = itemCount;

			if (index >= pagedList.TotalPage)
				index = pagedList.TotalPage;

			return index;
		}

		private static bool HasNextMore(PagedList pagedList, int itemCount = 5)
		{
			var end = GetEndNumber(pagedList, itemCount);

			return (end < pagedList.TotalPage);
		}

		private static bool HasPreviousMore(PagedList pagedList, int itemCount = 5)
		{
			var start = GetStartNumber(pagedList, itemCount);

			return (start > 1);
		}

		#endregion

		private int GetTotalPageCount() => Total % PageSize == 0 ? Total % PageSize : Total / PageSize + 1;

		public class PagedList
		{
			public int PageNumber { get; set; } = 1;
			public int PageSize { get; set; } = 10;
			public int TotalCount { get; set; }
			public int TotalPage { get; set; }

		}
	}
}
