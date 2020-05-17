using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Models.Account
{
	public class ExternalLoginViewModel
	{
		//public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }
	}
}
