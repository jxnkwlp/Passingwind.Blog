using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets
{
	public interface IWidgetsManager
	{
		Task<WidgetDescriptor> GetWidgetDescriptorAsync(string name, Guid id);
	}
}
