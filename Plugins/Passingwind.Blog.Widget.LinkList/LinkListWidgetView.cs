using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.LinkList.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.LinkList
{
	public class LinkListWidgetView : WidgetView
	{
		private readonly IPluginDataStore _pluginDataStore;

		public LinkListWidgetView(IPluginDataStore pluginDataStore)
		{
			_pluginDataStore = pluginDataStore;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
		{
			var list = await _pluginDataStore.GetListAsync<LinkModel>(Consts.Name, Descriptor.Id);

			return View(list);
		}
	}
}
