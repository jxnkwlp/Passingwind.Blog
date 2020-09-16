using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface IRoleFactory : IScopedDependency
	{
		RoleModel ToModel(Role entity, RoleModel model);
		Role ToEntity(RoleModel model, Role entity);
		Role ToEntity(RoleModel model);
	}
}
