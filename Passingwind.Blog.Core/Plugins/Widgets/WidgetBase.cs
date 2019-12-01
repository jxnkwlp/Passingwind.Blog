using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public abstract class WidgetBase : PluginBase, IWidget
	{
		public string ViewPath => "Views/Default.cshtml";

		protected virtual Task<object> GetViewDataAsync()
		{
			return Task.FromResult<object>(null);
		}

		public virtual Task<string> GetViewContentAsync()
		{
			return Task.FromResult<string>(string.Empty);
		}
	}
}
