using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets.WidgetComponents
{
	public interface IWidgetViewLocationResolveService
	{
		IEnumerable<string> Search(WidgetComponentDescriptor descriptor);
	}

}
