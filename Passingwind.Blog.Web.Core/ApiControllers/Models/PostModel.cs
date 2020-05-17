using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models
{
	public class PostApiListQueryModel : ApiListQueryModel
	{
		public bool IncludeUser { get; set; }
		public bool IncludeTags { get; set; }
		public bool IncludeCategory { get; set; }

		public string UserId { get; set; }
		public int? CategoryId { get; set; }
		public bool? IsDraft { get; set; }

		public IList<ApiListOrderQueryModel> Orders { get; set; }
	}

	public class PostApiGetQueryModel
	{
		public bool IncludeUser { get; set; }
		public bool IncludeTags { get; set; }
		public bool IncludeCategory { get; set; }
	}

	public class PostModel : BaseAuditTimeModel
	{
		[Required, MaxLength(512)]
		public string Title { get; set; }

		[Required, MaxLength(1024)]
		public string Slug { get; set; }

		[DataType(DataType.Html)]
		public string Content { get; set; }

		public string Description { get; set; }

		public bool IsDraft { get; set; }

		public int ViewsCount { get; set; }

		public int CommentsCount { get; set; }

		public bool EnableComment { get; set; } = true;

		public string UserId { get; set; }

		public UserModel User { get; set; }

		public DateTime PublishedTime { get; set; }

		public IList<CategoryModel> Categories { get; set; }
		public IList<string> Tags { get; set; }

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

	public class PostEditModel : BaseModel
	{
		[Required, MaxLength(512)]
		public string Title { get; set; }

		[Required, MaxLength(1024)]
		public string Slug { get; set; }

		[DataType(DataType.Html)]
		public string Content { get; set; }

		public string Description { get; set; }

		public bool IsDraft { get; set; }

		public bool EnableComment { get; set; } = true;

		public string UserId { get; set; }

		public DateTime PublishedTime { get; set; } = DateTime.Now;

		public IList<string> Tags { get; set; }

		public IList<int> Categories { get; set; }
		 
	}

	public class PostApiUpdateIsPublishedModel
	{
		public int[] Ids { get; set; }

		public bool Value { get; set; }
	}
}
