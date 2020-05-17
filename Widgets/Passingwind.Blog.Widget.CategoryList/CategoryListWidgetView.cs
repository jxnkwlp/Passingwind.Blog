using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.CategoryList.Models;
using Passingwind.Blog.Widgets;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CategoryList
{
	public class CategoryListWidgetView : WidgetComponent
	{
		private readonly ICategoryService _categoryService;

		public CategoryListWidgetView(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var list = (await _categoryService.GetListAsync())
						.OrderBy(t => t.DisplayOrder)
						.Select(entity => new CategoryViewModel()
						{
							Id = entity.Id,
							Description = entity.Description,
							DisplayOrder = entity.DisplayOrder,
							Name = entity.Name,
							Slug = entity.Slug,
						});

			var model = new WidgetViewModel()
			{
				List = list,
				Title = WidgetViewContext.ConfigurationInfo.Title,
			};

			return View(model);
		}

	}
}
