using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.CategoryList.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CategoryList
{
	public class CategoryListWidgetView : WidgetView
	{
		private readonly CategoryManager _categoryManager;

		public CategoryListWidgetView(CategoryManager categoryManager)
		{
			_categoryManager = categoryManager;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
		{
			var list = await _categoryManager.GetQueryable()
				.Select(entity => new CategoryViewModel()
				{
					Id = entity.Id,
					Description = entity.Description,
					DisplayOrder = entity.DisplayOrder,
					Name = entity.Name,
					Slug = entity.Slug,
				})
				.ToListAsync();

			return View(list);
		}
	}
}
