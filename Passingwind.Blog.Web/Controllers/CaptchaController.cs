using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Captcha;

namespace Passingwind.Blog.Web.Controllers
{
    public class CaptchaController : Controller
    {
        private CaptchaService _captchaService;

        public CaptchaController(CaptchaService captchaService)
        {
            _captchaService = captchaService;

        }

        public IActionResult Index()
        {
            var bytes = _captchaService.NewCaptchaImage();

            return File(bytes, "image/png");
        }
    }
}