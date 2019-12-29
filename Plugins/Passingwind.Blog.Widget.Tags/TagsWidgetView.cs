using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.Tags.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.Tags
{
	public class TagsWidgetView : WidgetView
	{
		private readonly TagsManager _tagsManager;

		public TagsWidgetView(TagsManager tagsManager)
		{
			_tagsManager = tagsManager;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
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

			return View(models);
		}
	}
}
