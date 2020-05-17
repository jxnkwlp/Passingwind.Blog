using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Web.ApiControllers.Models
{
	public class SendTestEmailRequestModel
	{
		[Required]
		public string Email { get; set; }

		public string Body { get; set; }
	}
}
