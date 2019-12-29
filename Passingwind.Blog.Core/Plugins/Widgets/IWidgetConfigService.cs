using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetConfigService
	{
		Task<Dictionary<string, IEnumerable<WidgetConfigInfo>>> GetAllAsync();
		Task<IEnumerable<WidgetConfigInfo>> GetByPositionAsync(string position);
		Task<WidgetConfigInfo> GetConfigInfoAsync(Guid id);
		Task AddAsync(string position, WidgetConfigInfo widgetConfigInfo);
		Task RemoveAsync(string position, Guid id);
		Task ClearAsync(string position);
		Task UpdateOrderAsync(string position, Guid id, int order);
	}
}
