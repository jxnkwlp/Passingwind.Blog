using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Passingwind.Blog
{
    public static class SettingManagerExtensions
    {
        public static IList<Setting> GetAllSettings(this SettingManager settingManager)
        {
            var task = settingManager.GetAllSettingsAsync();
            task.Wait();
            return task.Result;
        }


        public static T LoadSetting<T>(this SettingManager settingManager) where T : ISettings, new()
        {
            return AsyncHelper.RunSync<T>(() => settingManager.LoadSettingAsync<T>());

            //var task = settingManager.LoadSettingAsync<T>();
            //task.Wait();
            //return task.Result;
        }
    }
}
