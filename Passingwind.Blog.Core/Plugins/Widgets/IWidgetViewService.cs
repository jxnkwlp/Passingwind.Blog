namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetViewService
	{
		string RenderView(PluginDescriptor description, string viewPath);

		string RenderView<TModel>(PluginDescriptor description, string viewPath, TModel model);
	}
}