using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
    public class WidgetInstallContext
    {
        public IServiceProvider ServiceProvider { get; }

		public WidgetInstallContext(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}
    }
}
