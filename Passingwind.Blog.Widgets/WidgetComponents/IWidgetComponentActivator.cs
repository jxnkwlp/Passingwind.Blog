namespace Passingwind.Blog.Widgets
{
	/// <summary>
	///  create instance or release the widgetcomponent instance 
	/// </summary>
	public interface IWidgetComponentActivator
	{
		object Create(WidgetViewContext context);
		void Release(WidgetViewContext context, object viewComponent);
	}
}
