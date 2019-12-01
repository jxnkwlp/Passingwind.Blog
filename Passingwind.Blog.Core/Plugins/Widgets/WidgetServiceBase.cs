using System.IO;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public abstract class WidgetServiceBase : IWidgetService
	{
		public virtual string ViewPath => "Views/Default.cshtml";

		private readonly IPluginViewRenderService _pluginViewRenderService;

		public WidgetServiceBase(IPluginViewRenderService pluginViewRenderService)
		{
			_pluginViewRenderService = pluginViewRenderService;
		}

		public virtual async Task<string> GetViewContentAsync(PluginDescriptor pluginDescriptor)
		{
			var data = await GetViewDataAsync(pluginDescriptor);

			// build full view path. eg: {plugin name}/{viw path}
			string newViewPath = "/" + Path.Combine(pluginDescriptor.AssemblyName, ViewPath).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			var viewContent = await _pluginViewRenderService.RenderViewAsync(newViewPath, data);

			return viewContent;
		}

		public virtual Task<object> GetViewDataAsync(PluginDescriptor pluginDescriptor)
		{
			return Task.FromResult(default(object));
		}
	}
}
