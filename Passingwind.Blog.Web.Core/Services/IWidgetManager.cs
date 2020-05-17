using Passingwind.Blog.Widgets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
	public interface IWidgetManager : IWidgetConfigurationService
	{
		Task<Dictionary<string, IEnumerable<WidgetPositionConfigModel>>> GetZoneConfigListAsync();
		Task AddToZoneAsync(string zone, WidgetPositionConfigModel config);
		Task RemoveFromZoneAsync(string zone, WidgetPositionConfigModel config);
		Task ClearFromZoneAsync(string zone);
		Task RemoveAsync(string widgetId);

		Task InstallAsync(string id);
		Task UninstallAsync(string id);

		Task<IEnumerable<string>> GetInstalledAsync();
	}
}
