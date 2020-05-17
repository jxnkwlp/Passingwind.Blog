using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models
{
	public class TagsModel
	{
		[Required, MaxLength(32)]
		public string Name { get; set; }

		public int PostCount { get; set; }

		public int Id { get; set; }
	}

	public class TagsEditModel
	{
		[Required, MaxLength(32)]
		public string Name { get; set; }
	}

	public class TagsApiListQueryModel : ApiListQueryModel
	{
		public bool AllowPage { get; set; } = true;
	}
}
