using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets.TagHelpers
{
	[HtmlTargetElement("widget-zone")]
	[HtmlTargetElement(Attributes = "widget")]
	public class WidgetZoneTagHelper : TagHelper
	{
		[HtmlAttributeName("name")]
		public string Name { get; set; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		private readonly IWidgetConfigurationService _widgetConfigurationService;
		private readonly IWidgetViewInvoker _widgetViewInvoker;
		private readonly IWidgetComponentFactory _widgetComponentFactory;
		private readonly IWidgetContainer _widgetContainer;

		public WidgetZoneTagHelper(IWidgetConfigurationService widgetConfigurationService, IWidgetViewInvoker widgetViewInvoker, IWidgetComponentFactory widgetComponentFactory, IWidgetContainer widgetContainer)
		{
			_widgetConfigurationService = widgetConfigurationService;
			_widgetViewInvoker = widgetViewInvoker;
			_widgetComponentFactory = widgetComponentFactory;
			_widgetContainer = widgetContainer;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.Content.Clear();
			output.TagName = "";

			if (string.IsNullOrWhiteSpace(Name))
				throw new System.Exception("The name is must be set.");

			var widgetList = await _widgetConfigurationService.GetWidgetsByZoneAsync(Name);

			foreach (var item in widgetList.OrderBy(t => t.DisplayOrder))
			{
				await RenderWidgetViewAsync(item, output);
			}
		}

		private async Task RenderWidgetViewAsync(WidgetConfigurationModel configurationModel, TagHelperOutput output)
		{
			var widget = _widgetContainer.Widgets.FirstOrDefault(t => t.Id == configurationModel.WidgetId);

			if (widget == null)
			{
				output.TagName = "div";
				output.Content.SetContent($"The widget '{configurationModel.WidgetName}' not installed.");
				return;
			}

			var widgetViewContext = GetWidgetViewContext(widget, HtmlEncoder.Default);
			widgetViewContext.ConfigurationInfo = configurationModel;

			await _widgetViewInvoker.InvokeAsync(widgetViewContext);
		}

		private WidgetViewContext GetWidgetViewContext(WidgetDescriptor widget, HtmlEncoder htmlEncoder)
		{
			var viewDescriptior = _widgetComponentFactory.Create(widget);

			return new WidgetViewContext(viewDescriptior, null, ViewContext, htmlEncoder, ViewContext.Writer)
			{
				ComponentDescriptor = viewDescriptior,
			};
		}
	}
}
