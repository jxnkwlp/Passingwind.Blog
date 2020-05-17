using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IPageService : IService<Page>
	{
		Task<Page> FindBySlugAsync(string slug);
		IQueryable<Page> GetQueryable();

		Task<IPagedList<Page>> GetPagesPagedListAsync(ListBasicQueryInput input);
	}
}
