using Passingwind.Blog.Widgets;

namespace Passingwind.Blog.Widget.CustomHtml
{
	public class Widget : WidgetBase
	{
		public override string GetAdminConfigureUrl()
		{
			return "/admin/widgets/customhtml/configure";
		}
	}
}
