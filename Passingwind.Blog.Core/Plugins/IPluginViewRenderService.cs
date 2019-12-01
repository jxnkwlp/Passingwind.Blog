using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginViewRenderService
	{
		Task<string> RenderViewAsync(string viewPath);

		Task<string> RenderViewAsync<TModel>(string viewPath, TModel model);
	}
}