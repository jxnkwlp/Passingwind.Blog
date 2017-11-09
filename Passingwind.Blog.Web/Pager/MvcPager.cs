using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Passingwind.Blog.Pager;

namespace Passingwind.Blog.Web.Pager
{
    public class MvcPager : IHtmlContent
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        //private readonly IHtmlGenerator _htmlGenerator;
        protected IHtmlHelper _html;
        private IPagedList _pagedList;

        #region options

        private string _queryName = "page";
        private string _itemWrap = "";
        private string _pagerWrap = "";
        private string _firstText = "&lt;";
        private string _lastText = "&gt;";
        private string _prevText = "&laquo;";
        private string _nextText = "&raquo;";

        private bool _showPrevious = true;
        private bool _showNext = true;

        private bool _pageDiplayAuto = false;

        private string _pagerClasses = "pager";

        private bool _showPreviousMore = true;
        private bool _showNextMore = true;
        private string _previousMoreText = "...";
        private string _nextMoreText = "...";

        public MvcPager QueryName(string queryName)
        {
            _queryName = queryName;
            return this;
        }


        public MvcPager ItemWrap(string itemWrap)
        {
            _itemWrap = itemWrap;
            return this;
        }

        public MvcPager PagerWrap(string pagerWrap)
        {
            _pagerWrap = pagerWrap;
            return this;
        }

        public MvcPager PagerClass(string classNames)
        {
            _pagerClasses = classNames;
            return this;
        }

        public MvcPager ShowLast(bool value)
        {
            _showPrevious = value;
            return this;
        }

        public MvcPager ShowNext(bool value)
        {
            _showNext = value;
            return this;
        }

        public MvcPager PagerDisplayAuto(bool value)
        {
            _pageDiplayAuto = value;
            return this;
        }

        public MvcPager ShowNextMore(bool value)
        {
            _showNextMore = value;
            return this;
        }

        public MvcPager ShowPreviousMore(bool value)
        {
            _showPreviousMore = value;
            return this;
        }

        public MvcPager NextMoreText(string value)
        {
            _nextMoreText = value;
            return this;
        }

        public MvcPager PreviousMoreText(string value)
        {
            _previousMoreText = value;
            return this;
        }

        #endregion

        public MvcPager(IHtmlHelper html, IPagedList pagedList, IUrlHelperFactory urlHelperFactory)
        {
            this._html = html;
            this._pagedList = pagedList;
            this._urlHelperFactory = urlHelperFactory;

        }


        private string BuilderPager(string innerHtml)
        {
            var tag = new TagBuilder("div");

            if (!string.IsNullOrEmpty(_pagerWrap))
                tag = new TagBuilder(_pagerWrap);

            tag.AddCssClass(_pagerClasses);
            tag.InnerHtml.AppendHtml(innerHtml);

            return GetTagBuilderString(tag);
        }

        private string BuilderFirstItem()
        {
            var tag = BuildItemTag(1, _firstText);

            if (!_pagedList.HasFirst())
            {
                tag.AddCssClass("disabled");
            }

            return GetTagBuilderString(tag);
        }

        private string BuilderLastItem()
        {
            var tag = BuildItemTag(_pagedList.TotalPages, _lastText);

            if (!_pagedList.HasLast())
            {
                tag.AddCssClass("disabled");
            }

            return GetTagBuilderString(tag);
        }

        private string BuilderPreviousItem()
        {
            if (!_showPrevious)
                return null;

            var tag = BuildItemTag(_pagedList.GetPreviousNumber(), _prevText);

            if (!_pagedList.HasPrevious())
            {
                tag.AddCssClass("disabled");
            }

            return GetTagBuilderString(tag);
        }

        private string BuilderNextItem()
        {
            if (!_showNext)
                return null;

            var tag = BuildItemTag(_pagedList.GetNextNumber(), _nextText);

            if (!_pagedList.HasNext())
            {
                tag.AddCssClass("disabled");
            }

            return GetTagBuilderString(tag);
        }

        private string BuilderNumberItems()
        {
            if (_pageDiplayAuto && _pagedList.TotalPages <= 1)
            {
                return null;
            }

            var start = _pagedList.GetStartNumber();
            var end = _pagedList.GetEndNumber();

            StringBuilder sb = new StringBuilder();

            for (int i = start; i <= end; i++)
            {
                var tag = BuildItemTag(i, i.ToString());

                if (i == _pagedList.PageNumber)
                {
                    tag.AddCssClass("active");
                }

                sb.Append(GetTagBuilderString(tag));
            }

            return sb.ToString();
        }


        private string BuilderPreviousMoreItem()
        {
            if (_showPreviousMore && _pagedList.HasPreviousMore())
            {
                var tag = BuildItemTag(_pagedList.PageNumber, _previousMoreText);

                return GetTagBuilderString(tag);
            }

            return null;
        }

        private string BuilderNextMoreItem()
        {
            if (_showNextMore && _pagedList.HasNextMore())
            {
                var tag = BuildItemTag(_pagedList.PageNumber, _nextMoreText);

                return GetTagBuilderString(tag);
            }

            return null;
        }

        private TagBuilder BuildItemTag(int pageNumber, string text)
        {
            TagBuilder tag = new TagBuilder("a");
            if (pageNumber == _pagedList.PageNumber)
            {
                tag = new TagBuilder("span");
            }
            else
            {
                tag.Attributes.Add("href", GenerateUrl(pageNumber));
            }

            tag.InnerHtml.SetHtmlContent(text);

            if (!string.IsNullOrEmpty(_itemWrap))
            {
                TagBuilder tag2 = new TagBuilder(_itemWrap);

                tag2.InnerHtml.AppendHtml(tag);

                return tag2;
            }

            return tag;
        }

        private string GetTagBuilderString(TagBuilder tag)
        {
            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                tag.WriteTo(stringWriter, HtmlEncoder.Default);
                result = stringWriter.ToString();
            }

            return result;
        }

        protected RouteValueDictionary GetRouteValueDictionary(int pageNumber)
        {
            var routeValues = new RouteValueDictionary(); //_html.ViewContext.RouteData.Values;
            var queryStrings = _html.ViewContext.HttpContext.Request.Query;

            foreach (var key in queryStrings.Keys.Where(key => key != null))
            {
                var value = queryStrings[key];
                routeValues[key] = value;
            }

            if (pageNumber == 1 && _pagedList.PageNumber == 1)
            {

                if (routeValues.ContainsKey(_queryName))
                    routeValues.Remove(_queryName);
            }
            else
            {
                routeValues[_queryName] = pageNumber;
            }

            return routeValues;
        }

        private string GenerateUrl(int page)
        {
            var request = _html.ViewContext.HttpContext.Request;

            var qc = new QueryCollection();

            var queryString = new QueryString(); //QueryString.FromUriComponent(request.QueryString.ToUriComponent());

            foreach (var item in request.Query)
            {
                if (string.Equals(item.Key, _queryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    queryString = queryString.Add(item.Key, page.ToString());
                }
                else
                    queryString = queryString.Add(item.Key, item.Value);
            }

            if (!request.Query.ContainsKey(_queryName))
                queryString = queryString.Add(_queryName, page.ToString());

            return queryString.ToString();
        }

        //private string BuilderLink(int pageNumber)
        //{
        //    var routeValues = GetRouteValueDictionary(pageNumber);

        //    //var vpc = new VirtualPathContext(_html.ViewContext.HttpContext, null, routeValues);
        //    //var path = new  RouteBuilder( routes.GetVirtualPath(vpc).VirtualPath;

        //    var request = _html.ViewContext.HttpContext.Request;

        //    if (request.IsCategoryUrl())
        //    {
        //        return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("category", routeValues);
        //    }
        //    else if (request.IsTagsUrl())
        //    {
        //        return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("tags", routeValues);
        //    }
        //    else if (request.IsMonthListUrl())
        //    {
        //        return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("monthlist", routeValues);
        //    }
        //    else if (request.IsSearchUrl())
        //    {
        //        return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("search", routeValues);
        //    }
        //    else if (request.IsAuthorUrl())
        //    {
        //        return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("author", routeValues);
        //    }

        //    return _urlHelperFactory.GetUrlHelper(_html.ViewContext).Link("", routeValues);
        //}


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append(BuilderFirstItem());

            sb.Append(BuilderPreviousItem());

            sb.Append(BuilderPreviousMoreItem());

            sb.Append(BuilderNumberItems());

            sb.Append(BuilderNextMoreItem());

            sb.Append(BuilderNextItem());

            //sb.Append(BuilderLastItem());

            var result = BuilderPager(sb.ToString());

            return result.ToString();
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(this.ToString());
        }
    }
}
