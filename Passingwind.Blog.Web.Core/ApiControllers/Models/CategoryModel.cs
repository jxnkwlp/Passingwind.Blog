using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models
{
	public class CategoryModel : BaseModel
	{
		[Required, MaxLength(128)]
		public string Name { get; set; }

		public string Description { get; set; }

		public int? ParentId { get; set; }

		[Required, MaxLength(256)]
		public string Slug { get; set; }

		public int DisplayOrder { get; set; } = 1;
		 
	}

	public class CategoryListItemModel : CategoryModel
	{
		public int PostCount { get; set; } 
	}
}
