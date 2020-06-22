using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface IUserFactory : IScopedDependency
	{
		UserModel ToModel(User entity, UserModel model);
		User ToEntity(UserEditModel model, User entity);
		User ToEntity(UserEditModel model);
		User ToEntity(UserProfileUpdateModel model, User entity);
	}
}
