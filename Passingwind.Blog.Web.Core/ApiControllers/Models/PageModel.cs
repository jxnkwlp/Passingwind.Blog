using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models
{
	public class PageModel : BaseModel
	{
		[Required, MaxLength(128)]
		public string Title { get; set; }

		[Required, MaxLength(256)]
		public string Slug { get; set; }

		public string Content { get; set; }

		public string Keywords { get; set; }

		public string Description { get; set; }

		public string ParentId { get; set; }

		public bool IsFrontPage { get; set; }

		public bool IsShowInList { get; set; }

		public bool Published { get; set; } = true;

		public int DisplayOrder { get; set; }

		public bool IsMarkdownText
		{
			get
			{
				if (string.IsNullOrEmpty(Content))
					return false;

				return !Content.Contains("<p");
			}
		}
	}
}
