using System;
using System.Collections.Generic;
using System.Text;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public interface IWidgetViewFactory
	{
		object CreateViewComponent(WidgetViewContext context);
	}
}
