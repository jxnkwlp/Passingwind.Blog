using System;
using System.Reflection;

namespace Passingwind.Blog.Widgets
{
	public class WidgetComponentFactory : IWidgetComponentFactory
	{
		public WidgetComponentDescriptor Create(WidgetDescriptor widgetDescriptor)
		{
			var invokeMethod = widgetDescriptor.ComponentType.GetMethod("Invoke");
			if (invokeMethod == null)
				invokeMethod = widgetDescriptor.ComponentType.GetMethod("InvokeAsync");

			if (invokeMethod == null)
			{
				throw new Exception($"The Type '{widgetDescriptor.ComponentType.Name}' method Invoke or InvokeAsync not found. ");
			}

			return new WidgetComponentDescriptor()
			{
				TypeInfo = widgetDescriptor.ComponentType.GetTypeInfo(),
				MethodInfo = invokeMethod,
				Parameters = invokeMethod.GetParameters(),
				RootName = widgetDescriptor.Assembly.GetName().Name,
			};
		}
	}
}
