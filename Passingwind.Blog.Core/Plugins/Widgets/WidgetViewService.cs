using Microsoft.AspNetCore.Http;
using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetViewService : IWidgetViewService
	{
		private readonly IWidgetViewInvoker _widgetViewInvoker;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPluginViewRenderService _pluginViewRenderService;

		public WidgetViewService(IWidgetViewInvoker widgetViewInvoker, IHttpContextAccessor httpContextAccessor, IPluginViewRenderService pluginViewRenderService)
		{
			_widgetViewInvoker = widgetViewInvoker;
			_httpContextAccessor = httpContextAccessor;
			_pluginViewRenderService = pluginViewRenderService;
		}

		public async Task<string> GetViewContentAsync(WidgetDescriptor widgetDescriptor)
		{
			var viewContext = CreateWidgetViewContext(widgetDescriptor);

			var widgetViewResult = await _widgetViewInvoker.InvokeAsync(viewContext);

			if (widgetViewResult == null)
				return null;

			string viewPath = Path.Combine(widgetDescriptor.AssemblyName, widgetViewResult.ViewPath);

			if (widgetViewResult.ViewPath.StartsWith("/"))
				viewPath = widgetDescriptor.AssemblyName + widgetViewResult.ViewPath;

			if (widgetViewResult.ViewPath.StartsWith("~/"))
				viewPath = widgetDescriptor.AssemblyName + widgetViewResult.ViewPath.Substring(1);

			return await _pluginViewRenderService.RenderViewAsync(viewPath, widgetViewResult.ViewData);
		}

		protected WidgetViewContext CreateWidgetViewContext(WidgetDescriptor widgetDescriptor)
		{
			var contextArguments = new Dictionary<string, object>();
			return new WidgetViewContext(_httpContextAccessor.HttpContext, widgetDescriptor, contextArguments);
		}
	}
}
