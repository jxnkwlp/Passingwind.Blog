﻿using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;

namespace Passingwind.Blog.Widget.CustomHtml
{
	public class Widget : WidgetBase, IPluginConfigure
	{
		public void GetConfigureRouteData(out string controller, out string action)
		{
			controller = "CustomHtmlWidget";
			action = "Configure";
		}
	}
}
