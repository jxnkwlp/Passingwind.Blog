using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	public class Post : AuditTimeEntity
	{
		[Required, MaxLength(512)]
		public string Title { get; set; }

		[Required, MaxLength(256)]
		public string Slug { get; set; }

		public string Content { get; set; }

		public string Description { get; set; }

		public bool IsDraft { get; set; }

		public int ViewsCount { get; set; }

		public int CommentsCount { get; set; }

		public bool EnableComment { get; set; } = true;

		public string UserId { get; set; }

		public virtual User User { get; set; }

		public DateTime PublishedTime { get; set; } = DateTime.Now;

		public virtual ICollection<PostCategory> Categories { get; set; } = new List<PostCategory>();

		public virtual ICollection<PostTags> Tags { get; set; } = new List<PostTags>();

		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
	}
}