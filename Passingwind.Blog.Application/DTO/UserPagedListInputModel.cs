using Passingwind.Blog.Utils;
using System.Collections.Generic;

namespace Passingwind.Blog.Services.Models
{
	public class UserPagedListInputModel : ListBasicQueryInput
	{
		public bool IncludeRoles { get; set; }
		public bool IncludeRolePermissionKeys { get; set; }

		public Dictionary<string, FieldOrder> Orders { get; set; }
	}
}
