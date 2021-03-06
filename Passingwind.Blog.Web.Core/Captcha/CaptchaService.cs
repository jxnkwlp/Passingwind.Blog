using Microsoft.AspNetCore.Http;
using System;

namespace Passingwind.Blog.Web.Captcha
{
	public class CaptchaService : ICaptchaService
	{
		private readonly IHttpContextAccessor _contextAccessor;

		public CaptchaService(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		public bool Validate(string id, string input)
		{
			var realValue = _contextAccessor.HttpContext.Session.GetString($"_captcha_{id}");

			var result = string.Equals(input, realValue);

			if (result)
			{
				_contextAccessor.HttpContext.Session.Remove($"_captcha_{id}");
			}

			return result;
		}

		public CaptchaImageData GenerateCaptchaImage(string id, int length = 4)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			SkiaSharpCaptchaImage captchaImage = new SkiaSharpCaptchaImage();
			var code = captchaImage.NewRandomCode(length);

			var data = captchaImage.CreateImage(code);

			_contextAccessor.HttpContext.Session.SetString($"_captcha_{id}", code);

			return new CaptchaImageData()
			{
				Data = data,
				Id = id,
			};
		}
	}

}
