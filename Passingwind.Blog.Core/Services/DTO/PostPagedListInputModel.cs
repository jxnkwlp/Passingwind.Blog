using Passingwind.Blog.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passingwind.Blog.Services.Models
{
	public class PostIncludeOptions
	{
		public bool IncludeUser { get; set; }
		public bool IncludeTags { get; set; }
		public bool IncludeCategory { get; set; }
	}

	public class PostListInputModel : ListBasicQueryInput
	{
		public string UserId { get; set; }

		public int? CategoryId { get; set; }

		public int? TagsId { get; set; }

		public bool? IsDraft { get; set; }

		public DateTime? PublishedYearMonth { get; set; }
		public DateTime? PublishedDate { get; set; }

		public PostIncludeOptions IncludeOptions { get; set; }

		public Dictionary<string, FieldOrder> Orders { get; set; }
	}
}
