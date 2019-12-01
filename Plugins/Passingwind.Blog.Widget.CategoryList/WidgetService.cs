using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.CategoryList.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CategoryList
{
	public class WidgetService : WidgetServiceBase
	{
		private readonly CategoryManager _categoryManager;

		public WidgetService(IPluginViewRenderService pluginViewRenderService, CategoryManager categoryManager) : base(pluginViewRenderService)
		{
			_categoryManager = categoryManager;
		}

		public override Task<object> GetViewDataAsync(PluginDescriptor pluginDescriptor)
		{
			var list = _categoryManager.GetQueryable().Select(entity => new CategoryViewModel()
			{
				Id = entity.Id,
				Description = entity.Description,
				DisplayOrder = entity.DisplayOrder,
				Name = entity.Name,
				Slug = entity.Slug,
			})
			.ToList();

			return Task.FromResult<object>(list);
		}
	}
}
