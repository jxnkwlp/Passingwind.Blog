using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.PageList.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.PageList
{
	public class WidgetService : WidgetServiceBase
	{
		private readonly PageManager _pageManager;

		public WidgetService(IPluginViewRenderService pluginViewRenderService, PageManager pageManager) : base(pluginViewRenderService)
		{
			_pageManager = pageManager;
		}

		public override Task<object> GetViewDataAsync(PluginDescriptor pluginDescriptor)
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
