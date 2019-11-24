using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Captcha;

namespace Passingwind.Blog.Web.Controllers
{
	public class CaptchaController : Controller
	{
		private ICaptchaService _captchaService;

		public CaptchaController(ICaptchaService captchaService)
		{
			_captchaService = captchaService;
		}

		public IActionResult Index(string id)
		{
			var imageData = _captchaService.GenerateCaptchaImage(id);

			return File(imageData.Data, "image/png");
		}
	}
}