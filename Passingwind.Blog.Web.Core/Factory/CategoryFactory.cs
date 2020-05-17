using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class CategoryFactory : ICategoryFactory
	{
		private readonly IMapper _mapper;

		public CategoryFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public Category ToEntity(CategoryModel model, Category category)
		{
			return _mapper.Map(model, category);
		}

		public CategoryModel ToModel(Category category, CategoryModel model)
		{
			return _mapper.Map(category, model);
		}

		public CategoryListItemModel ToModel(Category category, CategoryListItemModel model)
		{
			model = _mapper.Map(category, model);

			return model;
		}
	}
}
