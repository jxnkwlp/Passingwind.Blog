using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.Tags.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.Tags
{
	public class WidgetService : WidgetServiceBase
	{
		private readonly TagsManager _tagsManager;

		public WidgetService(IPluginViewRenderService pluginViewRenderService, TagsManager tagsManager) : base(pluginViewRenderService)
		{
			_tagsManager = tagsManager;
		}

		public override async Task<object> GetViewDataAsync(PluginDescriptor pluginDescriptor)
		{
			var tagsList = _tagsManager.GetQueryable().ToList();

			var models = new List<TagsViewModel>();

			foreach (var item in tagsList)
			{
				models.Add(new TagsViewModel()
				{
					Name = item.Name,
					Count = await _tagsManager.GetPostsCountAsync(item.Id),
				});
			}

			return models;
		}
	}
}
