namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public interface IWidgetViewActivator
	{
		object Create(WidgetDescriptor widgetDescriptor);
	}
}
