using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
	/// <summary>
	///  Article entity class
	/// </summary>
	public class Post : AuditedEntity
	{
		[Required, MaxLength(512)]
		public string Title { get; set; }

		[Required, MaxLength(1024)]
		public string Slug { get; set; }

		public string Content { get; set; }

		public string Description { get; set; }

		public bool IsDraft { get; set; }

		public int ViewsCount { get; set; }

		public int CommentsCount { get; set; }

		public bool EnableComment { get; set; } = true;

		public string UserId { get; set; }

		public User User { get; set; }

		public DateTime PublishedTime { get; set; } = DateTime.Now;

		public IList<PostCategory> Categories { get; set; } = new List<PostCategory>();

		public IList<PostTags> Tags { get; set; } = new List<PostTags>();

		public IList<Comment> Comments { get; set; } = new List<Comment>();
	}
}