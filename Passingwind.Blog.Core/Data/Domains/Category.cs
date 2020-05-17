using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	/// <summary> 
	///  Category
	/// </summary>
	public class Category : AuditTimeEntity
	{
		[Required, MaxLength(128)]
		public string Name { get; set; }

		public string Description { get; set; }

		public int? ParentId { get; set; }

		[Required, MaxLength(256)]
		public string Slug { get; set; }

		public int DisplayOrder { get; set; } = 1;

		public virtual ICollection<PostCategory> Posts { get; set; } = new List<PostCategory>();
	}
}