namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetViewService
	{
		string RenderView(string viewName);
		string RenderView<TModel>(string viewName, TModel model);
	}
}