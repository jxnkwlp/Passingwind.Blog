using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models.Blog
{
	public class PostsViewModel
	{
		public IPagedList<PostModel> Posts { get; set; }

		public UserModel User { get; set; }

		public CategoryModel Category { get; set; }

		public TagsModel Tags { get; set; }
	}
}
