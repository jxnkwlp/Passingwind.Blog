using Passingwind.Blog.Data.Settings;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public static class SettingsExtensions
	{
		public static async Task<TSetttings> LoadAsync<TSetttings>(this ISettingService service) where TSetttings : ISettings, new()
		{
			var instance = Activator.CreateInstance<TSetttings>();
			var settingType = instance.GetType();
			var typeName = settingType.Name;

			var allSettings = await service.GetSettingsAsync(true);

			foreach (var prop in settingType.GetProperties())
			{
				if (!prop.CanRead || !prop.CanWrite)
					continue;

				var key = (typeName + "." + prop.Name).ToLower();

				var setting = allSettings.FirstOrDefault(t => t.Key == key);

				if (setting == null)
					continue;

				if (string.IsNullOrEmpty(setting.Value))
					continue;

				if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
					continue;
				if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting.Value))
					continue;

				object value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting.Value);

				prop.SetValue(instance, value);
			}

			return instance;
		}

		public static async Task SaveAsync<TSetttings>(
			this ISettingService service,
			TSetttings instance) where TSetttings : ISettings
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var settingType = instance.GetType();
			var typeName = settingType.Name;

			foreach (var prop in settingType.GetProperties())
			{
				if (!prop.CanRead || !prop.CanWrite)
					continue;

				var key = (typeName + "." + prop.Name).ToLower();

				if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
					continue;

				object value = prop.GetValue(instance, null);

				if (value == null)
				{
					await service.AddOrUpdateAsync(key, "");
				}
				else
				{
					string valueString = TypeDescriptor.GetConverter(prop.PropertyType).ConvertToInvariantString(value);

					await service.AddOrUpdateAsync(key, valueString);
				}
			}

			await service.ClearCacheAsync();
		}
	}
}
