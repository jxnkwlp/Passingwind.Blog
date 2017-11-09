using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class CommentManager
    {
        protected readonly EntityStore _store;

        public CommentManager(EntityStore store)
        {
            this._store = store;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            return await _store.CreateAsync(comment);
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            return await _store.UpdateAsync(comment);
        }

        public async Task DeleteAsync(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            await _store.DeleteAsync(comment);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var comment = await _store.FindByIdAsync<Comment>(new string[] { id });
            if (comment != null)
                await DeleteAsync(comment);
        }

        public Task<Comment> FindByIdAsync(string id)
        {
            return _store.FindByIdAsync<Comment>(new string[] { id });
        }

        public async Task<IList<Comment>> GetCommentsByPostId(string postId, bool onlyPublished)
        {
            var query = GetQueryable();

            if (onlyPublished)
                query = query.Where(t => t.IsApproved && !t.IsDeleted && !t.IsSpam);

            var result = query.Where(t => t.PostId.Equals(postId)).ToList();

            return await Task.FromResult(result);
        }

        public async Task<bool> IsTrustUserAsync(string email)
        {
            var query = GetQueryable().Where(t => t.Email.ToUpperInvariant() == email.ToUpperInvariant() && t.IsApproved && !t.IsSpam).Any();

            return await Task.FromResult(query);
        }

        public async Task SetIsDeletedAsync(string id)
        {
            var entity = await _store.Comments.FindAsync(id);

            if (entity != null)
            {
                if (!entity.IsDeleted)
                {
                    entity.IsDeleted = true;
                    await _store.UpdateAsync(entity);
                }
            }
        }

        public async Task UpdateIsApprovedAsync(string id, bool value)
        {
            var entity = await _store.Comments.FindAsync(id);

            if (entity != null)
            {
                if (entity.IsApproved != value)
                {
                    entity.IsApproved = value;
                    await _store.UpdateAsync(entity);
                }
            }
        }

        public async Task UpdateIsSpamAsync(string id, bool value)
        {
            var entity = await _store.Comments.FindAsync(id);

            if (entity != null)
            {
                if (entity.IsSpam != value)
                {
                    entity.IsSpam = true;
                    await _store.UpdateAsync(entity);
                }
            }
        }

        public IQueryable<Comment> GetQueryable()
        {
            return _store.Comments.OrderByDescending(t => t.CreationTime);
        }
    }
}