using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.PageList.Models;
using Passingwind.Blog.Widgets;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.PageList
{
	public class PageListWidgetView : WidgetComponent
	{
		private readonly IPageService _pageService;

		public PageListWidgetView(IPageService pageService)
		{
			_pageService = pageService;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var list = (await _pageService.GetListAsync(t => t.Published))
							.Select(entity => new PageModel()
							{
								Id = entity.Id,
								Content = entity.Content,
								ParentId = entity.ParentId,
								Description = entity.Description,
								Keywords = entity.Keywords,
								//IsShowInList = entity.IsShowInList,
								IsFrontPage = entity.IsFrontPage,
								Published = entity.Published,
								Slug = entity.Slug,
								Title = entity.Title,
								DisplayOrder = entity.DisplayOrder,
							});

			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
				List = list,
			};

			return View(model);
		}
	}
}
