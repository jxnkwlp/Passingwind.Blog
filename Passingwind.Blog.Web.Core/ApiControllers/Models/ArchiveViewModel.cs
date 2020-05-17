using System.Collections.Generic;

namespace Passingwind.Blog.Web.Models
{
	public class ArchiveViewModel
	{
		public Dictionary<CategoryListItemModel, IEnumerable<PostModel>> CategoryPosts { get; set; } = new Dictionary<CategoryListItemModel, IEnumerable<PostModel>>();

		public IEnumerable<PostModel> NoCategoryPosts { get; set; } = new List<PostModel>();


		public ArchiveViewModel()
		{
		}
	}

}
