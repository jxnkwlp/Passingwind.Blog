using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Data.Domains
{
	public class Role : IdentityRole<string>, IHasEntityCreationTime, IHasLastModificationTime, IEntity<string>
	{
		public const string AdministratorName = "Administrator";
		public const string EditorName = "Editor";
		public const string Anonymous = "Anonymous";

		public IList<RolePermission> Permissions { get; set; } = new List<RolePermission>();
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime? LastModificationTime { get; set; }

		public IList<UserRole> UserRoles { get; set; }

		public Role()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
