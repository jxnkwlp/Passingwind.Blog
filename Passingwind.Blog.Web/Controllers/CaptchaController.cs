using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Captcha;
using System;

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
			if (string.IsNullOrEmpty(id))
				return RedirectToAction(nameof(Index), new { id = Guid.NewGuid().ToString() });

			var imageData = _captchaService.GenerateCaptchaImage(id);

			return File(imageData.Data, "image/png");
		}
	}
}