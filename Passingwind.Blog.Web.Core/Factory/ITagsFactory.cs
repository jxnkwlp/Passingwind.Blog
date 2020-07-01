using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface ITagsFactory : IScopedDependency
	{
		TagsModel ToModel(Tags tags, TagsModel model);
	}
}
