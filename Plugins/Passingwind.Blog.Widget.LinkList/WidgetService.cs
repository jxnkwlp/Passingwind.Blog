using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.LinkList
{
	public class WidgetService : WidgetServiceBase
	{
		public WidgetService(IPluginViewRenderService pluginViewRenderService) : base(pluginViewRenderService)
		{
		}
	}
}
