using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Factory
{
	public interface ICommentFactory
	{
		CommentModel ToModel(Comment entity, CommentModel model);
	}
}
