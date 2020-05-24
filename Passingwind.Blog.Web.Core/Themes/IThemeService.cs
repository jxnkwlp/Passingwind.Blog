using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public interface IThemeService
	{
		Task<IEnumerable<ThemeDescriptor>> GetThemeDescriptorsAsync();

		Task SetDefaultThemeAsync(string name);

		Task<string> GetDefaultThemeAsync();
	}
}
