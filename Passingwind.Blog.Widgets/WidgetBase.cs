namespace Passingwind.Blog.Widgets
{
	public abstract class WidgetBase : IWidget
	{
		public virtual void ConfigureServices(WidgetConfigureServicesContext context)
		{
		}

		public virtual void Configure(WidgetConfigureContext context)
		{
		}

		public virtual string GetAdminConfigureUrl()
		{
			return null;
		}

		//public virtual Task InstallAsync(WidgetInstallContext context)
		//{
		//	return Task.CompletedTask;
		//}

		//public virtual Task UninstallAsync(WidgetUninstallContext context)
		//{
		//	return Task.CompletedTask;
		//}
	}
}
