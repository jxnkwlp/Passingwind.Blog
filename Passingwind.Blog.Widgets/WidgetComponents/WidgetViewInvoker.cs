using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public class WidgetViewInvoker : IWidgetViewInvoker
	{
		private readonly IWidgetComponentActivator _widgetComponentActivator;

		public WidgetViewInvoker(IWidgetComponentActivator widgetComponentActivator)
		{
			_widgetComponentActivator = widgetComponentActivator;
		}

		public async Task InvokeAsync(WidgetViewContext context)
		{
			await InvokeCoreAsync(context);
		}

		private async Task InvokeCoreAsync(WidgetViewContext context)
		{
			var executeMethod = context.ComponentDescriptor.MethodInfo;

			var methodReturnType = executeMethod.ReturnType;

			var instance = _widgetComponentActivator.Create(context) as WidgetComponent;
			instance.WidgetViewContext = context;

			IWidgetComponentViewResult result = null;

			if (methodReturnType == typeof(Task<IWidgetComponentViewResult>))
			{
				result = await MethodExecute<Task<IWidgetComponentViewResult>>(executeMethod, instance, null);
			}
			else if (methodReturnType == typeof(Task<string>))
			{
				result = new ContentWidgetViewResult(await MethodExecute<Task<string>>(executeMethod, instance, null));
			}
			else if (methodReturnType == typeof(IWidgetComponentViewResult))
			{
				result = MethodExecute<IWidgetComponentViewResult>(executeMethod, instance, null);
			}
			else
			{
				throw new Exception("Unsupport.");
			}

			await result.ExecuteAsync(context);
		}

		private TResult MethodExecute<TResult>(MethodInfo methodInfo, object instance, params object[] args)
		{
			var result = methodInfo.Invoke(instance, args);

			return (TResult)result;
		}
	}
}
