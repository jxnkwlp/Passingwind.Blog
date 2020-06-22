using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using System.Linq;

namespace Passingwind.Blog.Data
{
	public interface IPostRepository : IRepository<Post>, IScopedDependency
	{
		IQueryable<Post> GetPosts(bool includeAll = true);
	}
}
