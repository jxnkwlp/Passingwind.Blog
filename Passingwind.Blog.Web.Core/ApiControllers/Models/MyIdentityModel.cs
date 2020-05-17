using System.Collections.Generic;
using System.Security.Claims;

namespace Passingwind.Blog.Web.Models
{
	public class CurrentUserProfileModel
	{
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string AvatarUrl { get; set; }
		public string DisplayName { get; set; }
		public string UserDescription { get; set; }
		public string Bio { get; set; }

		public IEnumerable<string> Roles { get; set; }
		public IEnumerable<string> Permissions { get; set; }

		public string IdentityName { get; set; }
		public string AuthenticationType { get; set; }
		public IEnumerable<IdentityClaim> Claims { get; set; }
	}

	public class UserProfileUpdateModel
	{
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string AvatarUrl { get; set; }
		public string DisplayName { get; set; }
		public string UserDescription { get; set; }
		public string Bio { get; set; }
	}

	public class IdentityClaim
	{
		public string Type { get; set; }
		public string Value { get; set; }

		public IdentityClaim(string type, string value)
		{
			Type = type;
			Value = value;
		}
	}
}
