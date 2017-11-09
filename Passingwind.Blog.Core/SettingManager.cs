
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Passingwind.Blog.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class SettingManager
    {
        private const string CACHE_REGION = "Setting.";

        protected readonly EntityStore _store;

        private readonly IMemoryCache _memoryCache;


        public SettingManager(EntityStore store, IMemoryCache memoryCache)
        {
            this._store = store;
            this._memoryCache = memoryCache;
        }

        public async Task<Setting> CreateAsync(Setting Settings)
        {
            if (Settings == null)
                throw new ArgumentNullException(nameof(Settings));

            return await _store.CreateAsync(Settings);
        }

        public async Task<Setting> UpdateAsync(Setting Settings)
        {
            if (Settings == null)
                throw new ArgumentNullException(nameof(Settings));

            return await _store.UpdateAsync(Settings);
        }

        public async Task DeleteAsync(Setting Settings)
        {
            if (Settings == null)
                throw new ArgumentNullException(nameof(Settings));

            await _store.DeleteAsync(Settings);
        }

        public async Task DeleteByKeyAsync(string key)
        {
            var entity = await _store.FindByAsync<Setting>(t => t.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
            if (entity != null)
                await _store.DeleteAsync(entity);
        }

        public async Task<Setting> AddOrUpdateAsync(string key, string value)
        {
            var entity = await _store.FindByAsync<Setting>(t => t.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
            if (entity == null)
            {
                entity = new Setting()
                {
                    Key = key,
                    Value = value
                };

                await _store.CreateAsync(entity);
            }
            else
            {
                entity.Key = key;
                entity.Value = value;

                await _store.UpdateAsync(entity);
            }

            return entity;
        }

        public async Task<IList<Setting>> GetAllSettingsAsync()
        {
            var result = await _memoryCache.GetOrCreateAsync<IList<Setting>>(CACHE_REGION + "ALL", (s) =>
            {
                return Task.FromResult<IList<Setting>>(_store.Settings.ToList());
            });

            return result;
        }

        public async Task<T> LoadSettingAsync<T>() where T : ISettings, new()
        {
            string cacheKey = $"{CACHE_REGION}{typeof(T).Name}";

            var result = await _memoryCache.GetOrCreateAsync<T>(cacheKey, (s) =>
             {
                 var settings = Activator.CreateInstance<T>();
                 var typeName = typeof(T).Name;

                 var allSettings = _store.GetQueryable<Setting>().Where(t => t.Key.StartsWith(typeName)).ToList();

                 foreach (var prop in typeof(T).GetProperties())
                 {
                     if (!prop.CanRead || !prop.CanWrite)
                         continue;

                     var key = typeof(T).Name + "." + prop.Name;

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

                     prop.SetValue(settings, value, null);
                 }

                 return Task.FromResult(settings);
             });

            return result;
        }

        public async Task SaveSettingAsync<T>(T setting) where T : ISettings
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            string cacheKey = $"{CACHE_REGION}{typeof(T).Name}";

            //_store.AutoSaveChanges = false;

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                dynamic value = prop.GetValue(setting, null);

                if (value == null)
                {
                    await AddOrUpdateAsync(key, "");
                }
                else
                {
                    string valueStr = TypeDescriptor.GetConverter(prop.PropertyType).ConvertToInvariantString(value);

                    await AddOrUpdateAsync(key, valueStr);
                }
            }

            _memoryCache.Set(cacheKey, setting);

            //_store.AutoSaveChanges = true;
            //await _store.SaveChangesAsync(default(CancellationToken));
        }
    }
}