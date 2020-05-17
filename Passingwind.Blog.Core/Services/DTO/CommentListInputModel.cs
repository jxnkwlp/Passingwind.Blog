using Passingwind.Blog.Utils;
using System.Collections.Generic;

namespace Passingwind.Blog.Services.Models
{
	public class CommentListInputModel : ListBasicQueryInput
	{
		public CommentListIncludeOptions IncludeOptions { get; set; }

		public string Email { get; set; }
		public string Author { get; set; }
		public int? PostId { get; set; }

		public bool? Approved { get; set; }
		public bool? Spam { get; set; }

		public Dictionary<string, FieldOrder> Orders { get; set; }
	}

	public class CommentListIncludeOptions
	{
		public bool IncludePosts { get; set; }
	}
}
