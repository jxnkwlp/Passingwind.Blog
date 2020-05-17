using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Extensions
{
	public static class IdentityExtension
	{
		public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
		{
			var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
			if (claim == null) return null;

			return claim.Value;
		}
	}
}
