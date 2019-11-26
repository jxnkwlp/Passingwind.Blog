using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetsManager
	{
		Task<IEnumerable<string>> GetWidgetsAsync(string position);

		Task<string> GetViewContentAsync(string name);
	}
}
