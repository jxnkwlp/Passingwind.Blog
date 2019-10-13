using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
	/// <summary>
	///
	/// </summary>
	public class Category : Entity
	{
		[Required, MaxLength(128)]
		public string Name { get; set; }

		public string Description { get; set; }

		public string ParentId { get; set; }

		[Required, MaxLength(256)]
		public string Slug { get; set; }

		public int DisplayOrder { get; set; } = 1;

		public IList<PostCategory> Posts { get; set; } = new List<PostCategory>();
	}
}