using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.ApiControllers.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class CommonController : ApiControllerBase
	{
		private readonly IEmailSender _emailSender;

		public CommonController(IEmailSender emailSender)
		{
			_emailSender = emailSender;
		}

		[HttpPost("[action]")]
		public async Task SendTestEmailAsync([FromBody] SendTestEmailRequestModel model)
		{
			string html = $"======= TEST MESSAGE =======<br/><br/>{model.Body}";

			await _emailSender.SendEmailAsync(model.Email, "Test email", html);
		}
	}
}
