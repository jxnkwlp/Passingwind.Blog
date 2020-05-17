using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
    public class WidgetUninstallContext
    {
        public IServiceProvider ServiceProvider { get; }

		public WidgetUninstallContext(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}
    }
}
