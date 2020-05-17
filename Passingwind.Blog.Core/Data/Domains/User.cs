using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Data.Domains
{
	/// <summary>
	/// user entity class
	/// </summary>
	public class User : IdentityUser, IHasEntityCreationTime, IHasLastModificationTime, IEntity<string>
	{
		public const string DefaultAdministratorUserName = "Admin";

		public string DisplayName { get; set; }

		public string UserDescription { get; set; }

		public string Bio { get; set; }

		public string GetDisplayName()
		{
			if (string.IsNullOrEmpty(DisplayName))
				return Email;
			return DisplayName;
		}

		public bool IsLockouted
		{
			get
			{
				if (LockoutEnabled && LockoutEnd.HasValue)
				{
					var endTime = LockoutEnd.Value.UtcDateTime;

					return DateTimeOffset.UtcNow < endTime;
				}
				return false;
			}
		}

		public DateTime? LastModificationTime { get; set; }
		public DateTime CreationTime { get; set; }

		public virtual ICollection<UserRole> UserRoles { get; set; }

		public User()
		{
			Id = Guid.NewGuid().ToString();
			UserRoles = new List<UserRole>();
		}
	}
}
