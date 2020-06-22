using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ICategoryService : IService<Category>, IScopedDependency
	{
		IQueryable<Category> GetQueryable();
		Task<Category> GetBySlugAsync(string slug);
		Task<int> GetPostCountAsync(int categoryId, bool includeDraft = true);
	}
}
