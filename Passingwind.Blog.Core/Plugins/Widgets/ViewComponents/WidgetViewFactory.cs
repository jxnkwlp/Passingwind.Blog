namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public class WidgetViewFactory : IWidgetViewFactory
	{
		private readonly IWidgetViewActivator _widgetViewActivator;

		public WidgetViewFactory(IWidgetViewActivator widgetViewActivator)
		{
			_widgetViewActivator = widgetViewActivator;
		}

		public object CreateViewComponent(WidgetViewContext context)
		{
			var component = _widgetViewActivator.Create(context.Descriptor);

			InjectProperties(context, component as WidgetView);

			return component;
		}

		private void InjectProperties(WidgetViewContext context, WidgetView widgetView)
		{
			if (widgetView == null) return;

			widgetView.WidgetViewContext = context;
		}
	}
}
