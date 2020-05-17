using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class UserFactory : IUserFactory
	{
		private readonly IMapper _mapper;

		public UserFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public User ToEntity(UserEditModel model, User entity)
		{
			return _mapper.Map(model, entity);
		}

		public User ToEntity(UserEditModel model)
		{
			return ToEntity(model, new User());
		}

		public User ToEntity(UserProfileUpdateModel model, User entity)
		{
			return _mapper.Map(model, entity);
		}

		public UserModel ToModel(User entity, UserModel model)
		{
			return _mapper.Map(entity, model);
		}
	}
}
