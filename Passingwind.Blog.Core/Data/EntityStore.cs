using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Passingwind.Blog.Data
{
    public class EntityStore : IDisposable
    {
        private bool _disposed;

        protected DbContext Context { get; }

        public bool AutoSaveChanges { get; set; }

        public DbSet<Post> Posts { get { return Context.Set<Post>(); } }

        public DbSet<Page> Pages { get { return Context.Set<Page>(); } }

        public DbSet<Category> Categories { get { return Context.Set<Category>(); } }

        public DbSet<Tags> Tags { get { return Context.Set<Tags>(); } }

        public DbSet<PostCategory> PostCategories { get { return Context.Set<PostCategory>(); } }

        public DbSet<PostTags> PostTags { get { return Context.Set<PostTags>(); } }

        public DbSet<Comment> Comments { get { return Context.Set<Comment>(); } }

        public DbSet<Setting> Settings { get { return Context.Set<Setting>(); } }

        public EntityStore(DbContext context)
        {
            this.Context = context;
            this.AutoSaveChanges = true;
        }


        public virtual async Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            if (entity is IHasCreationTime creationTimeEntity && creationTimeEntity.CreationTime == default(DateTime))
            {
                creationTimeEntity.CreationTime = DateTime.Now;
            }

            Context.Add<T>(entity);

            await this.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual async Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            //if (!Context.Set<T>().Local.Contains(entity))
            //{
            //    //Context.Attach<T>(entity);
            //}

            if (entity is IHasModificationTime modificationTime)
            {
                modificationTime.LastModificationTime = DateTime.Now;
            }

            Context.Update<T>(entity);

            await this.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual async Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).IsDeleted = true;

                await UpdateAsync<T>(entity, cancellationToken);
                return;
            }

            Context.Remove<T>(entity);

            await this.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<T> FindByIdAsync<T>(object[] keys, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            return await Context.FindAsync<T>(keys, cancellationToken);
        }

        public virtual Task<T> FindByAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            var set = Context.Set<T>();

            var result = predicate == null ? set.FirstOrDefault() : set.FirstOrDefault(predicate);

            return Task.FromResult(result);
        }

        public virtual IQueryable<T> Includes<T>(params Expression<Func<T, object>>[] navigationPropertyPaths) where T : class
        {
            ThrowIfDisposed();

            var set = Context.Set<T>();

            var query = set.AsQueryable();

            foreach (var item in navigationPropertyPaths)
            {
                query = query.Include(item);
            }

            return query;
        }

        public virtual IQueryable<T> GetQueryable<T>() where T : class
        {
            ThrowIfDisposed();

            var set = Context.Set<T>();

            return set.AsQueryable();
        }


        public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (!this.AutoSaveChanges)
            {
                return Task.CompletedTask;
            }

            return this.Context.SaveChangesAsync(cancellationToken);
        }


        protected void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
