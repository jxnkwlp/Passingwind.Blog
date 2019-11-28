using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetViewService : IWidgetViewService
	{
		private string _pluginsPath = "~/Plugins/";
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;

		public WidgetViewService(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
		{
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
		}

		protected IView FindView(PluginDescriptor description, string viewPath, ActionContext actionContext)
		{
			string viewFullPath = Path.Combine(description.RelativePath, viewPath).Replace(@"\", "/");

			var path = _viewEngine.GetAbsolutePath(viewFullPath, viewPath);

			//var viewEngineResult = _viewEngine.GetView(description.VirtualPath + "/", viewPath, false);

			var viewEngineResult = _viewEngine.GetView(null, viewFullPath, true);
			//var viewEngineResult = _viewEngine.GetView(null, "/Views/Home/Test.cshtml", true);

			if (!viewEngineResult.Success)
				viewEngineResult = _viewEngine.FindView(actionContext, viewPath, false);

			if (!viewEngineResult.Success)
			{
				throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewPath));
			}

			return viewEngineResult.View;
		}

		public string RenderView(PluginDescriptor description, string viewPath)
		{
			var actionContext = GetActionContext();

			var view = FindView(description, viewPath, actionContext);

			using (var output = new StringWriter())
			{
				var viewContext = new ViewContext(
					actionContext,
					view,
					new ViewDataDictionary(
						metadataProvider: new EmptyModelMetadataProvider(),
						modelState: new ModelStateDictionary()),
					new TempDataDictionary(
						actionContext.HttpContext,
						_tempDataProvider),
					output,
					new HtmlHelperOptions());

				view.RenderAsync(viewContext).GetAwaiter().GetResult();

				return output.ToString();
			}
		}

		public string RenderView<TModel>(PluginDescriptor description, string viewPath, TModel model)
		{
			var actionContext = GetActionContext();

			var view = FindView(description, viewPath, actionContext);

			using (var output = new StringWriter())
			{
				var viewContext = new ViewContext(
					actionContext,
					view,
					new ViewDataDictionary<TModel>(
						metadataProvider: new EmptyModelMetadataProvider(),
						modelState: new ModelStateDictionary())
					{
						Model = model
					},
					new TempDataDictionary(
						actionContext.HttpContext,
						_tempDataProvider),
					output,
					new HtmlHelperOptions());

				view.RenderAsync(viewContext).GetAwaiter().GetResult();

				return output.ToString();
			}
		}

		private ActionContext GetActionContext()
		{
			var httpContext = new DefaultHttpContext();
			httpContext.RequestServices = _serviceProvider;
			return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
		}
	}
}
