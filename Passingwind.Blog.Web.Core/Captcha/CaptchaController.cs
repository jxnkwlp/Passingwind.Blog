using Microsoft.AspNetCore.Mvc;
using System;

namespace Passingwind.Blog.Web.Captcha
{
	public class CaptchaController : Controller
	{
		private readonly ICaptchaService _captchaService;

		public CaptchaController(ICaptchaService captchaService)
		{
			_captchaService = captchaService;
		}

		[HttpGet("captcha/image", Name = "_captcha_image")]
		public IActionResult Image(string id)
		{
			if (string.IsNullOrEmpty(id))
				return RedirectToAction(nameof(Image), new { id = Guid.NewGuid().ToString() });

			var imageData = _captchaService.GenerateCaptchaImage(id);

			return File(imageData.Data, "image/png");
		}

	}
}
