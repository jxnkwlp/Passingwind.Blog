using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.TagsCloud.Models;
using Passingwind.Blog.Widgets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.TagsCloud
{
	public class TagsCloudWidgetView : WidgetComponent
	{
		private readonly ITagsService _tagsService;

		public TagsCloudWidgetView(ITagsService tagsService)
		{
			_tagsService = tagsService;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var tagsList = await _tagsService.GetListAsync(new Services.DTO.TagsListInputModel()
			{
				IncludePosts = true,
			});

			var models = new List<TagsModel>();

			foreach (var item in tagsList)
			{
				models.Add(new TagsModel()
				{
					Name = item.Name,
					Count = item.Posts?.Count() ?? 0,
				});
			}

			var model = new WidgetViewModel()
			{
				Title = WidgetViewContext.ConfigurationInfo.Title,
				List = models,
			};

			return View(model);
		}
	}
}
