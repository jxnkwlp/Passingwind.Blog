using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
	public class Role : IdentityRole<string>
	{
		public const string AdministratorName = "Administrator";
		public const string EditorName = "Editor";
		public const string Anonymous = "Anonymous";

		public IList<RolePermission> Permissions { get; set; } = new List<RolePermission>();

		public Role()
		{
			this.Id = Guid.NewGuid().ToString();
		}
	}

	public class RolePermission
	{
		[Required]
		public string PermissionKey { get; set; }

		public string RoleId { get; set; }

		public Role Role { get; set; }
	}
}