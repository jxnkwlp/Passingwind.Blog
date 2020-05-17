using Passingwind.Blog.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	public interface IPostRepository : IRepository<Post>
	{
		IQueryable<Post> GetPosts(bool includeAll = true);
	}
}
