using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.Models.Account
{
	public class ExternalLoginConfirmationViewModel
	{
		public string ReturnUrl { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
		  
		[Required] 
		public string LoginProvider { get; set; }
	}
}
