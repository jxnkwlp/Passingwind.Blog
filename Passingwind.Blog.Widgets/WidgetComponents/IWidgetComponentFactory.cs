namespace Passingwind.Blog.Widgets
{
	public interface IWidgetComponentFactory
	{
		WidgetComponentDescriptor Create(WidgetDescriptor widgetDescriptor); 
	}
}
