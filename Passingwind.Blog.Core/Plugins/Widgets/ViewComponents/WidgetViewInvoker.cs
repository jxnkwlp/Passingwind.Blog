using Passingwind.Blog.Internal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public class WidgetViewInvoker : IWidgetViewInvoker
	{
		private readonly IWidgetViewFactory _widgetViewFactory;
		private readonly DiagnosticListener _diagnosticListener;

		public WidgetViewInvoker(IWidgetViewFactory widgetViewFactory, DiagnosticListener diagnosticListener)
		{
			_widgetViewFactory = widgetViewFactory;
			_diagnosticListener = diagnosticListener;
		}

		public async Task<IWidgetViewResult> InvokeAsync(WidgetViewContext context)
		{
			var component = _widgetViewFactory.CreateViewComponent(context) as WidgetView;

			if (component == null)
				return null;

			return await component.InvokeAsync();
		}

		//protected MethodInfo FindInvokeMethod(object instance)
		//{
		//	var invokeMethod = instance.GetType().GetMethods().FirstOrDefault(t => t.IsPublic && (t.Name == "InvokeAsync" || t.Name == "Invoke"));

		//	if (invokeMethod == null)
		//		throw new Exception($"Can't find InvokeAsync or Invoke Method in {instance.GetType().FullName}. ");

		//	var returnType = invokeMethod.ReturnType;

		//	if (returnType == typeof(void) || returnType == typeof(Task))
		//	{
		//		throw new Exception($"Invoke Method must return value.");
		//	}


		//	return invokeMethod;
		//}

		//protected IWidgetViewResult MethodExecute(MethodInfo methodInfo)
		//{
		//	var executor = ObjectMethodExecutor.Create(methodInfo, null);

		//	return null;
		//}
	}
}
