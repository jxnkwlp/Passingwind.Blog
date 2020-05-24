using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public class ThemeAccessor : IThemeAccessor
	{
		private readonly IThemeService _themeService;

		public ThemeAccessor(IThemeService themeService)
		{
			_themeService = themeService;
		}

		public async Task<string> GetCurrentThemeNameAsync()
		{
			return await _themeService.GetDefaultThemeAsync();
		}
	}
}
