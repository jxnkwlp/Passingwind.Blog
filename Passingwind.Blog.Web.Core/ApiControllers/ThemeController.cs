using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Passingwind.Blog.Web.ApiControllers.Models;
using Passingwind.Blog.Web.Themes;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class ThemeController : ApiControllerBase
	{
		private readonly IHostEnvironment _hostEnvironment;
		private readonly IThemeService _themeService;

		public ThemeController(IHostEnvironment hostEnvironment, IThemeService themeService)
		{
			_hostEnvironment = hostEnvironment;
			_themeService = themeService;
		}

		[HttpGet]
		public async Task<ThemeListModel> GetListAsync()
		{
			var list = await _themeService.GetThemeDescriptorsAsync();
			var currentTheme = await _themeService.GetDefaultThemeAsync();

			return new ThemeListModel()
			{
				Name = string.IsNullOrWhiteSpace(currentTheme) ? "Default" : currentTheme,
				Themes = list.Select(t => t.Description)
			};
		}

		[HttpPost("default")]
		public async Task SetDefaultAsync(string name)
		{
			await _themeService.SetDefaultThemeAsync(name);
		}

	}
}
