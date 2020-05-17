using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IRazorViewService
	{
		Task<string> RenderViewAsync<TModel>(string viewName, TModel model) where TModel : class;
		Task<string> RenderViewAsync(string viewName);
	}
}
