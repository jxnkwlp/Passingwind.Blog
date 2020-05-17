using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Models
{
	public class UserApiPagedListQueryModel : ApiListQueryModel
	{
		public bool IncludeRoles { get; set; }

		public IList<ApiListOrderQueryModel> Orders { get; set; }
	}

	public class UserModel : BaseAuditTimeModel<string>
	{
		public virtual DateTimeOffset? LockoutEnd { get; set; }

		public virtual bool PhoneNumberConfirmed { get; set; }

		public virtual string PhoneNumber { get; set; }

		public virtual bool EmailConfirmed { get; set; }

		public virtual string Email { get; set; }

		public virtual string UserName { get; set; }

		public virtual bool LockoutEnabled { get; set; }

		public string DisplayName { get; set; }

		public string UserDescription { get; set; }

		public string Bio { get; set; }

		public IEnumerable<RoleModel> Roles { get; set; }
	}

	public class UserEditModel : BaseModel<string>
	{
		public virtual bool EmailConfirmed { get; set; }

		public virtual string Email { get; set; }

		public virtual string UserName { get; set; }

		public virtual bool LockoutEnabled { get; set; }

		public string DisplayName { get; set; }

		public string UserDescription { get; set; }

		public string Bio { get; set; }

		public string Password { get; set; }

		public IList<string> RoleIds { get; set; }
	}

	public class UserSetLockInputModel
	{
		public string[] UserIds { get; set; }

		public bool Value { get; set; }

		public DateTime? LockEnd { get; set; }
	}
}
