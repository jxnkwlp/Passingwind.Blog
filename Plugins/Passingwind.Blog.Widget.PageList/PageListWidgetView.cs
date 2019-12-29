using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.PageList.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.PageList
{
	public class PageListWidgetView : WidgetView
	{
		private readonly PageManager _pageManager;

		public PageListWidgetView(PageManager pageManager)
		{
			_pageManager = pageManager;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
		{
			var list = await _pageManager.GetQueryable().Select(entity => new PageViewModel()
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
			.ToListAsync();

			return View(list);
		}
	}
}
