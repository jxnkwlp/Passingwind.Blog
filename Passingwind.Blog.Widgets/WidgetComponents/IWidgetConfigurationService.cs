using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public interface IWidgetConfigurationService
	{
		Task<IEnumerable<WidgetConfigurationModel>> GetWidgetsAsync();
		Task<IEnumerable<WidgetConfigurationModel>> GetWidgetsByZoneAsync(string name);
	}
}
