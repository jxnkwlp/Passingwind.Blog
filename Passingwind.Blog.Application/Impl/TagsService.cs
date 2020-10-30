using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.DTO;
using Passingwind.PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class TagsService : Service<Tags>, ITagsService
	{
		public TagsService(IRepository<Tags, int> Repository) : base(Repository)
		{
		}

		public async Task DeleteByNameAsync(string name)
		{
			var entity = await Repository.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
			if (entity != null)
				await DeleteAsync(entity);
		}

		public async Task<Tags> GetByNameAsync(string name)
		{
			return await Repository.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
		}

		public async Task<Tags> GetOrCreateAsync(string name)
		{
			var entity = await Repository.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

			if (entity == null)
			{
				entity = new Tags()
				{
					Name = name,
				};

				await Repository.InsertAsync(entity);
			}

			return entity;
		}

		public async Task<IPagedList<Tags>> GetTagsPagedListAsync(TagsListInputModel input)
		{
			var query = Repository.GetQueryable();

			if (input.IncludePosts)
				query = Repository.Includes(t => t.Posts);

			query = query
						  .OrderBy(t => t.Name)
						  .WhereIf(t => t.Name.Contains(input.SearchTerm), () => !string.IsNullOrWhiteSpace(input.SearchTerm));

			return await query.ToPagedListAsync(input);
		}

		public async Task<IEnumerable<Tags>> GetListAsync(TagsListInputModel input)
		{
			var query = Repository.GetQueryable();

			if (input.IncludePosts)
				query = Repository.Includes(t => t.Posts);

			query = query
						  .OrderBy(t => t.Name)
						  .WhereIf(t => t.Name.Contains(input.SearchTerm), () => !string.IsNullOrWhiteSpace(input.SearchTerm));

			return await query.ToListAsync();
		}

	}
}
