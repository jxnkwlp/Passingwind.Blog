using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class PageService : Service<Page>, IPageService
	{
		private readonly ISlugService _slugService;

		public PageService(IRepository<Page, int> Repository, ISlugService slugService) : base(Repository)
		{
			_slugService = slugService;
		}

		public override async Task InsertAsync(Page entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.InsertAsync(entity, cancellationToken);
		}

		public override async Task UpdateAsync(Page entity, CancellationToken cancellationToken = default)
		{
			entity.Slug = await _slugService.NormalarAsync(entity.Slug);
			await base.UpdateAsync(entity, cancellationToken);
		}

		public async Task<Page> FindBySlugAsync(string slug)
		{
			return await Repository.FirstOrDefaultAsync(t => t.Slug.ToUpper() == slug.ToUpper());
		}

		public Task<IPagedList<Page>> GetPagesPagedListAsync(ListBasicQueryInput input)
		{
			var query = Repository.GetQueryable()
									.OrderBy(t => t.DisplayOrder)
									.ThenByDescending(t => t.CreationTime)
									.WhereIf(t => t.Title.Contains(input.SearchTerm), () => !string.IsNullOrWhiteSpace(input.SearchTerm));

			return query.ToPagedListAsync(input);
		}

		public IQueryable<Page> GetQueryable()
		{
			return Repository.GetQueryable().OrderBy(t => t.DisplayOrder);
		}
	}
}
