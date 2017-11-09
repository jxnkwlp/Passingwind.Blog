using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class TagsManager
    {
        protected readonly EntityStore _store;

        public TagsManager(EntityStore store)
        {
            this._store = store;
        }

        public async Task<Tags> CreateAsync(Tags tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            return await _store.CreateAsync(tags);
        }

        public async Task<Tags> UpdateAsync(Tags tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            return await _store.UpdateAsync(tags);
        }

        public async Task DeleteAsync(Tags tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            await _store.DeleteAsync(tags);
        }

        public async Task DeleteByNameAsync(string tagsName)
        {
            var tags = await FindByNameAsync(tagsName);
            if (tags != null)
                await DeleteAsync(tags);
        }

        public async Task<Tags> FindByNameAsync(string tagsName)
        {
            return await _store.FindByAsync<Tags>(t => t.Name.Equals(tagsName, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<Tags> CreateOrUpdateAsync(string tagsName)
        {
            if (string.IsNullOrWhiteSpace(tagsName))
                throw new ArgumentNullException(nameof(tagsName));

            var entity = _store.Tags.FirstOrDefault(t => t.Name.Equals(tagsName, StringComparison.CurrentCultureIgnoreCase));

            if (entity == null)
            {
                return await _store.CreateAsync(new Tags() { Name = tagsName });
            }
            else
            {
                entity.Name = tagsName;

                return await _store.UpdateAsync(entity);
            }
        }

        public async Task<IList<PostTags>> GetPosts(string tagsId)
        {
            var query = _store.PostTags.Where(t => t.TagsId == tagsId).ToList();

            return await Task.FromResult<IList<PostTags>>(query);
        }

        public async Task<int> GetPostsCountAsync(string tagsId)
        {
            var query = _store.PostTags.Where(t => t.TagsId == tagsId).Count();

            return await Task.FromResult(query);
        }

        public IQueryable<Tags> GetQueryable()
        {
            return _store.Tags
                .Include(t => t.Posts)
                .OrderBy(t => t.Name);
            //return _store.Tags.OrderBy(t => t.Name);
        }
    }
}