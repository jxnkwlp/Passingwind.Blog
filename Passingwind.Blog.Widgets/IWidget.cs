using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public interface IWidget
	{
		void ConfigureServices(WidgetConfigureServicesContext context);
		void Configure(WidgetConfigureContext context);

		//Task InstallAsync(WidgetInstallContext context);
		//Task UninstallAsync(WidgetUninstallContext context);

		string GetAdminConfigureUrl();
	}
}
