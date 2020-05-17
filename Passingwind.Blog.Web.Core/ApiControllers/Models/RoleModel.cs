using System.Collections.Generic;

namespace Passingwind.Blog.Web.Models
{
	public class RoleApiListQueryModel : ApiListQueryModel
	{
		public bool IncludePermissionKeys { get; set; }
	}

	public class RoleModel : BaseModel<string>
	{
		public string Name { get; set; }

		public IList<string> PermissionKeys { get; set; }
	}
}
