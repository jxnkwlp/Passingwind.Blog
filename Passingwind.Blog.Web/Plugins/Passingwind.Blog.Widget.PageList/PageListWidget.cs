using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.PageList.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.PageList
{
	public class PageListWidget : WidgetBase, IWidget, IPlugin
	{
		private readonly PageManager _pageManager;

		public PageListWidget(IWidgetViewService widgetViewService, PageManager pageManager) : base(widgetViewService)
		{
			_pageManager = pageManager;
		}

		protected override Task<object> GetViewDataAsync()
		{
			var list = _pageManager.GetQueryable().Select(entity => new PageViewModel()
			{
				Id = entity.Id,
				Content = entity.Content,
				ParentId = entity.ParentId,
				Description = entity.Description,
				Keywords = entity.Keywords,
				IsShowInList = entity.IsShowInList,
				IsFrontPage = entity.IsFrontPage,
				Published = entity.Published,
				Slug = entity.Slug,
				Title = entity.Title,
				DisplayOrder = entity.DisplayOrder,
			})
			.ToList();

			return Task.FromResult<object>(list);
		}

	}
}
