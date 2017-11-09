using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class PostManager
    {
        protected readonly EntityStore _store;

        public PostManager(EntityStore store)
        {
            this._store = store;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            return await _store.CreateAsync(post);
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            return await _store.UpdateAsync(post);
        }

        public async Task DeleteAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            await _store.DeleteAsync(post);
        }

        public async Task DeleteByIdAsync(string postId)
        {
            var post = await _store.FindByIdAsync<Post>(new string[] { postId });
            if (post != null)
                await DeleteAsync(post);
        }

        public async Task<Post> FindByIdAsync(string postId)
        {
            // ef core 没有延迟加载
            return await _store.Posts
                .Include(t => t.User)
                .Include(t => t.Categories).ThenInclude(c => c.Category)
                .Include(t => t.Tags).ThenInclude(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == postId);
        }

        public async Task<Post> FindBySlugAsync(string slug)
        {
            // ef core 没有延迟加载
            return await _store.Posts
                  .Include(t => t.User)
                  .Include(t => t.Categories).ThenInclude(c => c.Category)
                  .Include(t => t.Tags).ThenInclude(t => t.Tags)
                  .FirstOrDefaultAsync(t => t.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        }

        public async Task UpdateIsPublishAsync(string postId, bool published)
        {
            var post = await _store.FindByIdAsync<Post>(new string[] { postId });
            if (post != null)
            {
                post.IsDraft = !published;

                await _store.UpdateAsync(post);
            }
        }

        public async Task<int> IncreaseViewCountAsync(string postId)
        {
            var post = await FindByIdAsync(postId);
            if (post != null)
                post.ViewsCount++;

            await UpdateAsync(post);

            return post.CommentsCount;
        }

        #region Category

        public Task<IList<PostCategory>> GetPostCategoriesAsync(string postId)
        {
            return Task.FromResult<IList<PostCategory>>(_store.GetQueryable<PostCategory>().Where(t => t.PostId == postId).ToList());
        }

        public Task<IList<Category>> GetCategoriesAsync(string postId)
        {
            var categoryIds = _store.GetQueryable<PostCategory>().Where(t => t.PostId == postId).Select(t => t.CategoryId);

            return Task.FromResult<IList<Category>>(_store.GetQueryable<Category>().Where(t => categoryIds.Contains(t.Id)).ToList());
        }

        public async Task RemoveCategoryAsync(Post post, string categoryId)
        {
            var postCategory = await _store.FindByAsync<PostCategory>(t => t.CategoryId == categoryId && t.PostId == post.Id);
            if (postCategory != null)
            {
                await _store.DeleteAsync(postCategory);
            }
        }

        public async Task RemoveCategoryAsync(PostCategory postCategory)
        {
            if (postCategory != null)
            {
                await _store.DeleteAsync(postCategory);
            }
        }

        #endregion Category

        #region tags

        public Task<IList<string>> GetTagsStringListAsync(string postId)
        {
            var tagsIds = _store.GetQueryable<PostTags>().Where(t => t.PostId == postId).Select(t => t.TagsId).ToList();

            var query = _store.GetQueryable<Tags>().Where(t => tagsIds.Contains(t.Id)).Select(t => t.Name).ToList();

            return Task.FromResult<IList<string>>(query);
        }

        public Task<IList<PostTags>> GetTagsAsync(string postId)
        {
            return Task.FromResult<IList<PostTags>>(_store.GetQueryable<PostTags>().Where(t => t.PostId == postId).ToList());
        }

        public async Task RemoveTagsAsync(Post post, string tagId)
        {
            var postTags = await _store.FindByAsync<PostTags>(t => t.TagsId == tagId && t.PostId == post.Id);
            if (postTags != null)
            {
                await _store.DeleteAsync(postTags);
            }
        }

        #endregion tags

        #region Comments

        public async Task<int> IncreaseCommentsCountAsync(string postId)
        {
            var post = await FindByIdAsync(postId);
            if (post != null)
                post.CommentsCount++;

            await UpdateAsync(post);

            return post.CommentsCount;
        }

        #endregion Comments

        public IQueryable<Post> GetQueryable()
        {
            return _store.Posts
                .Include(t => t.User)
                .Include(t => t.Categories).ThenInclude(c => c.Category)
                .Include(t => t.Tags).ThenInclude(t => t.Tags)
                .OrderByDescending(t => t.CreationTime).ThenByDescending(t => t.PublishedTime);
        }
    }
}