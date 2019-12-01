using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetsManager
	{
		Task<IEnumerable<WidgetConfigInfo>> GetWidgetsAsync(string position);

		Task<string> GetViewContentAsync(string name);
	}
}
