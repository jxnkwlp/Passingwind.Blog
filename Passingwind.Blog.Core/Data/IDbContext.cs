using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
    /// <summary>
    ///
    /// </summary>
    public interface IDbContext
    {
        IQueryable<Post> Posts { get; }
        IQueryable<Page> Pages { get; }
        IQueryable<Category> Categories { get; }
        IQueryable<Tags> Tags { get; }
        IQueryable<User> Users { get; }

        T Add<T>(T entity);
    }
}
