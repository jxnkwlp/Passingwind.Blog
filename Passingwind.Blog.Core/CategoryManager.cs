using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class CategoryManager
    {
        protected readonly EntityStore _store;

        public CategoryManager(EntityStore store)
        {  
            this._store = store;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return await _store.CreateAsync(category);
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return await _store.UpdateAsync(category);
        }

        public async Task DeleteAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _store.DeleteAsync(category);
        }

        public async Task DeleteByIdAsync(string categoryId)
        {
            var category = await _store.FindByIdAsync<Category>(new string[] { categoryId });
            if (category != null)
                await DeleteAsync(category);
        }

        public Task<Category> FindByIdAsync(string categoryId)
        {
            return _store.FindByIdAsync<Category>(new string[] { categoryId });
        }

        public Task<Category> GetBySlugAsync(string slug)
        {
            return Task.FromResult(_store.GetQueryable<Category>().FirstOrDefault(t => t.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase)));
        }

        public Task<int> GetPostCountAsync(string id, bool withDraft = true)
        {
            var query = _store.GetQueryable<PostCategory>();
            if (!withDraft)
            {
                query = query.Where(t => !t.Post.IsDraft);
            }

            return Task.FromResult(query.Count(t => t.CategoryId == id));
        }

        public IQueryable<Category> GetQueryable()
        {
            return _store.Categories
                .Include(t => t.Posts)
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.Name);
            // return _store.Categories.OrderBy(t => t.DisplayOrder).ThenBy(t => t.Name);
        }
    }
}