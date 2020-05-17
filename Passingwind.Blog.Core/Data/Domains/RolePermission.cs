using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	public class RolePermission
	{
		[Required, MaxLength(128)]
		public string Key { get; set; }

		public string RoleId { get; set; }

		public virtual Role Role { get; set; }
	}
}