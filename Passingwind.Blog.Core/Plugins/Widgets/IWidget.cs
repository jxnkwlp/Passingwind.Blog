using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidget : IPlugin
	{
		string ViewPath { get; }

		Task<string> GetViewContentAsync();
	}
}
