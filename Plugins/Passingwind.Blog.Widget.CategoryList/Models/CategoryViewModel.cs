namespace Passingwind.Blog.Widget.CategoryList.Models
{
	public class CategoryViewModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Slug { get; set; }

		public int DisplayOrder { get; set; } = 1;

	}
}
