using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class PageManager
    {
        protected readonly EntityStore _store;

        public PageManager(EntityStore store)
        {
            this._store = store;
        }

        public async Task<Page> CreateAsync(Page page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            return await _store.CreateAsync(page);
        }

        public async Task<Page> UpdateAsync(Page page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            
            return await _store.UpdateAsync(page);
        }

        public async Task DeleteAsync(Page page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            await _store.DeleteAsync(page);
        }

        public async Task DeleteByIdAsync(string pageId)
        {
            var page = await _store.FindByIdAsync<Page>(new string[] { pageId });
            if (page != null)
                await DeleteAsync(page);
        }

        public async Task<Page> FindByIdAsync(string pageId)
        {
            return await _store.FindByIdAsync<Page>(new string[] { pageId });
        }

        public async Task<Page> FindBySlugAsync(string slug)
        {
            return await _store.GetQueryable<Page>()
                .FirstOrDefaultAsync(t => t.Slug.ToUpper() == slug.ToUpper());
        }

        public IQueryable<Page> GetQueryable()
        {
            return _store.Pages.OrderBy(t => t.DisplayOrder);
        }
    }
}