using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;
using System.Linq;

namespace Passingwind.Blog.Web.Factory
{
	public class PostFactory : IPostFactory
	{
		private readonly IMapper _mapper;

		private readonly IUserFactory _userFactory;
		private readonly ICategoryFactory _categoryFactory;

		public PostFactory(IMapper mapper, IUserFactory userFactory, ICategoryFactory categoryFactory)
		{
			_mapper = mapper;
			_userFactory = userFactory;
			_categoryFactory = categoryFactory;
		}

		public Post ToEntity(PostEditModel model, Post post)
		{
			return _mapper.Map(model, post);
		}

		public Post ToEntity(PostEditModel model)
		{
			return ToEntity(model, new Post());
		}

		public PostModel ToModel(Post entity, PostModel model)
		{
			model = _mapper.Map(entity, model);

			if (entity.User != null)
				model.User = _userFactory.ToModel(entity.User, new UserModel());

			if (entity.Categories != null)
				model.Categories = entity.Categories.Select(t => _categoryFactory.ToModel(t.Category, new CategoryModel())).ToArray();

			if (entity.Tags != null)
				model.Tags = entity.Tags.Select(t => t.Tags.Name).ToArray();

			return model;
		}

		public PostEditModel ToModel(Post entity, PostEditModel model)
		{
			return _mapper.Map(entity, model);
		}
	}
}
