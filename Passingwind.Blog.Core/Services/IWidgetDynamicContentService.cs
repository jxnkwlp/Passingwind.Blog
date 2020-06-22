using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Widgets;
using Passingwind.Blog.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IWidgetDynamicContentService : IService<WidgetDynamicContent>, IScopedDependency
	{
		Task<T> GetAsync<T>(Guid widgetId, string userId) where T : IWidgetDynamicContent;

		Task<IEnumerable<T>> GetListAsync<T>(Guid widgetId, string userId) where T : IWidgetDynamicContent;

		Task<WidgetDynamicContent> InsertAsync(IWidgetDynamicContent content);
		Task<WidgetDynamicContent> UpdateAsync(IWidgetDynamicContent content); 

		Task DeleteAsync(Guid widgetId, string userId);
		Task DeleteAsync(Guid widgetId, string userId, Dictionary<string, object> properties);
	}
}
