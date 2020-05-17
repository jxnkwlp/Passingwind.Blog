using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data.Domains;
using System.Linq;

namespace Passingwind.Blog.Data
{
	public class PostRepository : Repository<Post>, IPostRepository
	{
		public PostRepository(DbContext dbContext) : base(dbContext)
		{
		}

		public virtual IQueryable<Post> GetPosts(bool includeAll = true)
		{
			if (includeAll)
				return Entities.Include(t => t.User)
							 .Include(t => t.Categories).ThenInclude(c => c.Category)
							 .Include(t => t.Tags).ThenInclude(t => t.Tags)
							 .OrderByDescending(t => t.CreationTime)
							 .ThenByDescending(t => t.PublishedTime);
			return Entities;
		}
	}
}
