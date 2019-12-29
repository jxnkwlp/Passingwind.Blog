using Passingwind.Blog.Plugins.Widgets.ViewComponents;
using Passingwind.Blog.Widget.CustomHtml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CustomHtml
{
	public class CustomHtmlWidgetView : WidgetView
	{
		private readonly IPluginDataStore _pluginDataStore;

		public CustomHtmlWidgetView(IPluginDataStore pluginDataStore)
		{
			_pluginDataStore = pluginDataStore;
		}

		public override async Task<IWidgetViewResult> InvokeAsync()
		{
			var data = (await _pluginDataStore.GetListAsync<CustomHtmlModel>(Consts.Name, this.Descriptor.Id)).FirstOrDefault();

			if (data == null)
				return View(new CustomHtmlModel());

			return View(data);
		}
	}
}
