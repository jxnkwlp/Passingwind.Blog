using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface ICommentFactory : IScopedDependency
	{
		CommentModel ToModel(Comment entity, CommentModel model);
	}
}
