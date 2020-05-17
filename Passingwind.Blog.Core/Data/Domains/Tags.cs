using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	public class Tags : Entity
	{
		[Required, MaxLength(32)]
		public string Name { get; set; }

		public virtual ICollection<PostTags> Posts { get; set; } = new List<PostTags>();
	}
}