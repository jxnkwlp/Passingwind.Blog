using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Widgets;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Passingwind.Blog.Services
{
	public static class WidgetDynamicContentExtensions
	{
		private static readonly ConcurrentDictionary<string, PropertyInfo[]> _cache = new ConcurrentDictionary<string, PropertyInfo[]>();

		public static T ToDynamicContent<T>(this WidgetDynamicContent content) where T : IWidgetDynamicContent
		{
			string key = typeof(T).FullName;

			var properties = _cache.GetOrAdd(key, (_) =>
				 {
					 return typeof(T).GetProperties().Where(t => t.CanWrite && t.CanRead).ToArray();
				 });

			var instance = Activator.CreateInstance<T>();
			instance.Id = content.Id;
			instance.UserId = content.UserId;
			instance.WidgetId = content.WidgetId;

			foreach (var item in properties)
			{
				var name = item.Name;
				if (name == "Id" || name == "UserId" || name == "WidgetId")
					continue;

				var valueString = content.Properties?.FirstOrDefault(t => t.Name == name)?.Value;

				if (string.IsNullOrEmpty(valueString))
					continue;

				if (!TypeDescriptor.GetConverter(item.PropertyType).CanConvertFrom(typeof(string)))
					continue;
				if (!TypeDescriptor.GetConverter(item.PropertyType).IsValid(valueString))
					continue;

				object value = TypeDescriptor.GetConverter(item.PropertyType).ConvertFromInvariantString(valueString);

				item.SetValue(instance, value);
			}

			return instance;
		}

		public static WidgetDynamicContent ToContent(this IWidgetDynamicContent instance)
		{
			string key = instance.GetType().FullName;

			var properties = _cache.GetOrAdd(key, (_) =>
			{
				return instance.GetType().GetProperties().Where(t => t.CanWrite && t.CanRead).ToArray();
			});

			var entity = new WidgetDynamicContent()
			{
				Id = instance.Id,
				UserId = instance.UserId,
				WidgetId = instance.WidgetId,
			};


			foreach (var item in properties)
			{
				var name = item.Name;
				if (name == "Id" || name == "UserId" || name == "WidgetId")
					continue;

				var value = item.GetValue(instance);

				if (value == null)
					continue;

				entity.Properties.Add(new WidgetDynamicContentProperty()
				{
					Name = name,
					Value = value.ToString(),
					ValueType = item.PropertyType.Name, 
				});
			}

			return entity;
		}

		//public static async Task<T> Get<T>(this IWidgetDynamicContentService service, Guid widgetId, string userId)
		//{
		//	var entity = await service.GetAsync(widgetId, userId);

		//	if (entity == null)
		//		return default;

		//	return entity.ToContentData<T>();
		//}

		//public static Task<WidgetDynamicContent> InsertAsync<T>(this IWidgetDynamicContentService service, Guid widgetId, string userId, T instance)
		//{

		//}

		//public static Task<WidgetDynamicContent> UpdateAsync<T>(this IWidgetDynamicContentService service, Guid widgetId, string userId, T instance)
		//{

		//}


	}
}
