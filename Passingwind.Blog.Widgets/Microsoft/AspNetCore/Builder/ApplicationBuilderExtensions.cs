using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Widgets;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Microsoft.AspNetCore.Builder
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
	public static class ApplicationBuilderExtensions
	{
		public static void UseWidgets(this IApplicationBuilder app)
		{
			var application = app.ApplicationServices.GetRequiredService<IWidgetFactory>();

			application.Initialize(app);
		}
	}
}
