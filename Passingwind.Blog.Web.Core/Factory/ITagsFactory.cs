using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Factory
{
	public interface ITagsFactory
	{
		TagsModel ToModel(Tags tags, TagsModel model);
	}
}
