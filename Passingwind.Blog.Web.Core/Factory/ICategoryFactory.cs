using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface ICategoryFactory
	{
		CategoryModel ToModel(Category category, CategoryModel model); 
		CategoryListItemModel ToModel(Category category, CategoryListItemModel model);
		Category ToEntity(CategoryModel model, Category category);
	}
}
