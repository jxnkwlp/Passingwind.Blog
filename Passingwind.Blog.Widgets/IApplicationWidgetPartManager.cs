using System.Reflection;

namespace Passingwind.Blog.Widgets
{
	public interface IApplicationWidgetPartManager
	{
		void Add(Assembly assembly);
		void Remove(Assembly assembly);
	}
}
