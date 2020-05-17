using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface IPostFactory
	{
		PostModel ToModel(Post entity, PostModel model);
		PostEditModel ToModel(Post entity, PostEditModel model);
		Post ToEntity(PostEditModel model, Post post);
		Post ToEntity(PostEditModel model);
	}
}
