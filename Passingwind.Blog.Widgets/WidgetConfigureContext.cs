using System;

namespace Passingwind.Blog.Widgets
{
	public class WidgetConfigureContext
	{
		public IServiceProvider ServiceProvider { get; }

		public WidgetConfigureContext(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));

			ServiceProvider = serviceProvider;
		}
	}
}
