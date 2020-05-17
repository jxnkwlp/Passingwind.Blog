using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface IRoleFactory
	{
		RoleModel ToModel(Role entity, RoleModel model);
		Role ToEntity(RoleModel model, Role entity);
		Role ToEntity(RoleModel model);
	}
}
