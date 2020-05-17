using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services.DTO;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ITagsService : IService<Tags>
	{
		Task<Tags> GetByNameAsync(string name);
		Task DeleteByNameAsync(string name);
		Task<Tags> GetOrCreateAsync(string name);

		Task<IEnumerable<Tags>> GetListAsync(TagsListInputModel input);
		Task<IPagedList<Tags>> GetTagsPagedListAsync(TagsListInputModel input);
	}
}
