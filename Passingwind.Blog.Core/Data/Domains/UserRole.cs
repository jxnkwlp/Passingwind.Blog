using Microsoft.AspNetCore.Identity;

namespace Passingwind.Blog.Data.Domains
{
	public class UserRole : IdentityUserRole<string>
	{
		public virtual User User { get; set; }

		public virtual Role Role { get; set; }
	}
}
