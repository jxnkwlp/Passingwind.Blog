using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models.Account
{
	public class LoginViewModel
	{
		[Required]
		[MaxLength(32)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MaxLength(32)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
		 
	}
}
