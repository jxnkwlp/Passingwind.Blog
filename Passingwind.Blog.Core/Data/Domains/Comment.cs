using System;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	/// <summary>
	///  the post commend
	/// </summary>
	public class Comment : AuditTimeEntity
	{
		public Guid GuidId { get; set; } = Guid.NewGuid();

		[MaxLength(16)]
		public string Category { get; set; } = "post";

		public int PostId { get; set; }

		public virtual Post Post { get; set; }

		[MaxLength(64)]
		public string UserId { get; set; }

		[MaxLength(32)]
		public string Author { get; set; }

		[MaxLength(128)]
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

		/// <summary>
		///  删除
		/// </summary>
		public bool IsDeleted { get; set; }

		public int? ParentId { get; set; }

		[MaxLength(32)]
		public string IP { get; set; }
	}
}
