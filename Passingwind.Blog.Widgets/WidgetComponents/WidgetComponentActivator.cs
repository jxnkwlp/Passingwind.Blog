using Passingwind.Blog.Widgets.Infrastructure;
using System;

namespace Passingwind.Blog.Widgets
{
	internal class WidgetComponentActivator : IWidgetComponentActivator
	{
		private readonly ITypeActivatorCache _typeActivatorCache;

		public WidgetComponentActivator(ITypeActivatorCache typeActivatorCache)
		{
			_typeActivatorCache = typeActivatorCache;
		}

		public object Create(WidgetViewContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var componentType = context.ComponentDescriptor.TypeInfo;

			if (componentType == null)
				throw new ArgumentNullException(nameof(context.ComponentDescriptor.TypeInfo));

			var viewComponent = _typeActivatorCache.CreateInstance<object>(
				context.ViewContext.HttpContext.RequestServices,
				context.ComponentDescriptor.TypeInfo.AsType());

			return viewComponent;
		}

		public void Release(WidgetViewContext context, object viewComponent)
		{
			if (viewComponent is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
