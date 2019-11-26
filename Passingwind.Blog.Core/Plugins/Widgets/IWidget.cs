using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidget : IPlugin
	{
		string ViewName { get; }

		Task<string> GetViewContentAsync();
	}
}
