using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public class ThemeFactory : IThemeContainer, IThemeFactory
	{
		private const string _rootName = "Themes";

		public ThemeFactory(IServiceCollection services, IHostEnvironment hostEnvironment)
		{
			Services = services;
			HostEnvironment = hostEnvironment;

			LoadThemes();

			services.AddSingleton<IThemeContainer>(this);
			services.AddSingleton<IThemeFactory>(this);

			services.AddScoped<IThemeAccessor, ThemeAccessor>();
			services.AddScoped<IThemeService, ThemeService>();
		}

		public IReadOnlyCollection<ThemeDescriptor> Themes { get; private set; }
		public IServiceCollection Services { get; }
		public IHostEnvironment HostEnvironment { get; }

		public void Initialize(IApplicationBuilder applicationBuilder)
		{
			RegisterThemeContentPath(applicationBuilder);
		}

		public Task ReloadAsync()
		{
			LoadThemes();
			return Task.CompletedTask;
		}

		private void LoadThemes()
		{
			string rootPath = Path.Combine(HostEnvironment.ContentRootPath, _rootName);

			Themes = ThemeLoader.Load(rootPath);
		}

		private void RegisterThemeContentPath(IApplicationBuilder app)
		{
			var logger = app.ApplicationServices.GetRequiredService<ILogger<ThemeFactory>>();

			foreach (var item in Themes)
			{
				if (Directory.Exists(item.ContentRootPath))
				{
					var requestPath = $"/themes/{item.Name.ToLower()}";
					app.UseStaticFiles(new StaticFileOptions()
					{
						RequestPath = new Microsoft.AspNetCore.Http.PathString(requestPath),
						FileProvider = new PhysicalFileProvider(item.ContentRootPath),
					});

					logger.LogDebug($"Use '{requestPath}' as theme content root path.");
				}
			}
		}
	}
}
