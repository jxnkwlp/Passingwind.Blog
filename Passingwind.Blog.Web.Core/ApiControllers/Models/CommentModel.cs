using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models
{
	public class CommentApiListQueryModel : ApiListQueryModel
	{
		public string Email { get; set; }
		public string Author { get; set; }
		public int? PostId { get; set; }
		public bool? Approved { get; set; }
		public bool? Spam { get; set; }
	}

	public class CommentModel : BaseAuditTimeModel
	{
		public int PostId { get; set; }

		public PostModel Post { get; set; }

		[MaxLength(64)]
		public string UserId { get; set; }

		//public UserModel User { get; set; }

		[MaxLength(32)]
		public string Author { get; set; }

		[MaxLength(256)]
		public string Email { get; set; }

		public string Website { get; set; }

		public string Content { get; set; }

		public string Country { get; set; }

		/// <summary>
		///  已审核
		/// </summary>
		public bool IsApproved { get; set; } = true;

		/// <summary>
		///  垃圾
		/// </summary>
		public bool IsSpam { get; set; }

		public string ParentId { get; set; }

		[MaxLength(32)]
		public string IP { get; set; }
	}

	public class CommentApprovedUpdateModel
	{
		public int Id { get; set; }
		public bool Value { get; set; }
	}

	public class CommentSpamUpdateModel
	{
		public int Id { get; set; }
		public bool Value { get; set; }
	}

	public class CommentReplayModel
	{
		public int CommentId { get; set; }

		public string Content { get; set; } 
	}
}
