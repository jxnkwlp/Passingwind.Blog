using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Captcha
{
    public class CaptchaService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CaptchaService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;

        }

        public bool Validate(string input)
        {
            var realValue = _contextAccessor.HttpContext.Session.GetString("_Captcha_");

            return string.Equals(input, realValue);
        }

        public byte[] NewCaptchaImage(int length = 4)
        {
            CaptchaImage captchaImage = new CaptchaImage();
            var code = captchaImage.NewRandomCode();

            _contextAccessor.HttpContext.Session.SetString("_Captcha_", code);

            return captchaImage.CreateImage(code);
        }
    }
}
