using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Services;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class SettingsController : AdminControllerBase
    {
        private readonly SettingManager _settingManager;
        private readonly IEmailSender _emailSender;

        public SettingsController(SettingManager settingManager, IEmailSender emailSender)
        {
            this._settingManager = settingManager;

            this._emailSender = emailSender;
        }

        public async Task<IActionResult> Basic()
        {
            var settings = await _settingManager.LoadSettingAsync<BasicSettings>();

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Basic(BasicSettings model)
        {
            if (ModelState.IsValid)
            {
                await _settingManager.SaveSettingAsync(model);

                AlertSuccess("已保存。");
            }

            return View(model);
        }


        public async Task<IActionResult> Advanced()
        {
            var settings = await _settingManager.LoadSettingAsync<AdvancedSettings>();

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Advanced(AdvancedSettings model)
        {
            if (ModelState.IsValid)
            {
                await _settingManager.SaveSettingAsync(model);

                AlertSuccess("已保存。");
            }

            return View(model);
        }


        public async Task<IActionResult> Comments()
        {
            var settings = await _settingManager.LoadSettingAsync<CommentsSettings>();

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Comments(CommentsSettings model)
        {
            if (ModelState.IsValid)
            {
                await _settingManager.SaveSettingAsync(model);

                AlertSuccess("已保存。");
            }

            return View(model);
        }

        public async Task<IActionResult> Email()
        {
            var settings = await _settingManager.LoadSettingAsync<EmailSettings>();

            return View(nameof(Email), settings);
        }

        [HttpPost]
        public async Task<IActionResult> Email(EmailSettings model)
        {
            if (ModelState.IsValid)
            {
                await _settingManager.SaveSettingAsync(model);

                AlertSuccess("已保存。");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailTest(string sendto)
        {
            if (!string.IsNullOrEmpty(sendto))
            {
                var email = await _settingManager.LoadSettingAsync<EmailSettings>();

                try
                {
                    await _emailSender.SendEmailAsync(sendto, "this is test", "this is a test email.");

                    AlertSuccess("已发送！");
                }
                catch (Exception ex)
                {
                    AlertError(ex.Message);
                }

            }

            return await Email();
        }
    }
}
