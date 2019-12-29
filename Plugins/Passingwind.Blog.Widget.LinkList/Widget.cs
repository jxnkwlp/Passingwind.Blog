using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;

namespace Passingwind.Blog.Widget.LinkList
{
	public class Widget : WidgetBase, IPluginConfigure
	{
		public void GetConfigureRouteData(out string controller, out string action)
		{
			//area = "LinkList";
			controller = "LinkListWidget";
			action = "Configure";
		}

	}
}
