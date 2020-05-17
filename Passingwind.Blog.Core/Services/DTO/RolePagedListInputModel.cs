using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Models
{
	public class RolePagedListInputModel : ListBasicQueryInput
	{
		public bool IncludePermissionKeys { get; set; }
	}
}
