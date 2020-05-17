using Passingwind.Blog.Widgets;

namespace Passingwind.Blog.Widget.LinkList
{
	public class Widget : WidgetBase
	{
		public override string GetAdminConfigureUrl()
		{
			return "/admin/widgets/linklist/configure";
		}

		public override void ConfigureServices(WidgetConfigureServicesContext context)
		{
			base.ConfigureServices(context);
		}

		public override void Configure(WidgetConfigureContext context)
		{
		}
	}
}
