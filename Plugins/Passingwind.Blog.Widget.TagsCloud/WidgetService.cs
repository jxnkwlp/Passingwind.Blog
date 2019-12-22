using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Widget.TagsCloud.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.TagsCloud
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
			var tagsList = await _tagsManager.GetQueryable().Include(t => t.Posts).ToListAsync();

			var models = new List<TagsViewModel>();

			foreach (var item in tagsList)
			{
				models.Add(new TagsViewModel()
				{
					Name = item.Name,
					Count = item.Posts.Count(),
				});
			}

			return models;
		}
	}
}
