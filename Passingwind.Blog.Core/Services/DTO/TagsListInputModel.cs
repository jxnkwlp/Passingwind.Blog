using Passingwind.Blog.Services.Models;

namespace Passingwind.Blog.Services.DTO
{
	public class TagsListInputModel : ListBasicQueryInput
	{
		public bool IncludePosts { get; set; }
	}
}
