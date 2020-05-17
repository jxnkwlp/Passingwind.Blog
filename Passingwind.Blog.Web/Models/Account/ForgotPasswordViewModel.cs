using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models.Account
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

	}
}
