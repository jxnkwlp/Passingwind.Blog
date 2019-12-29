using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public class PluginViewRenderService : IPluginViewRenderService
	{
		private string _pluginsPath = "~/Plugins/";
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IRazorPageFactoryProvider _razorPageFactoryProvider;
		private readonly ICompositeViewEngine _compositeViewEngine;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PluginViewRenderService(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IWebHostEnvironment hostEnvironment, IRazorPageFactoryProvider razorPageFactoryProvider, ICompositeViewEngine compositeViewEngine, IHttpContextAccessor httpContextAccessor)
		{
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
			_hostEnvironment = hostEnvironment;
			_razorPageFactoryProvider = razorPageFactoryProvider;
			_compositeViewEngine = compositeViewEngine;
			_httpContextAccessor = httpContextAccessor;
		}

		//protected IView FindViewTest(PluginDescriptor description, string viewPath, ActionContext actionContext)
		//{
		//	try
		//	{
		//		string viewFullPath = "" + Path.Combine(description.RelativePath, viewPath).Replace(@"\", "/");

		//		//var result1 = _razorPageFactoryProvider.CreateFactory("/Plugins/Passingwind.Blog.Widget.PageList/Views/Default.cshtml");

		//		var result5 = _compositeViewEngine.FindView(actionContext, "/Views/Default.cshtml", true);

		//		var result2 = _compositeViewEngine.GetView(null, "/email/test.cshtml", true);

		//		var result4 = _viewEngine.GetView(null, "/Views/Default.cshtml", true);

		//		var result3 = _viewEngine.GetView(null, viewFullPath, true);

		//	}
		//	catch (Exception ex)
		//	{

		//	}


		//	//var result1 = _viewEngine2.GetView(null, "~/email/default.a/test.cshtml", true);
		//	//var result2 = _viewEngine2.GetView(null, "~/email/abc/test.cshtml", true);

		//	//var result3 = _viewEngine2.GetView(null, "~/plugins/test.cshtml", true);
		//	//var result4 = _viewEngine2.GetView(null, "~/plugins/aaa/test.cshtml", true);

		//	return null;

		//	//var path = _viewEngine.GetAbsolutePath(viewFullPath, viewPath);

		//	////var viewEngineResult = _viewEngine.GetView(description.VirtualPath + "/", viewPath, false);

		//	//var viewEngineResult = _viewEngine.GetView(null, viewFullPath, true);
		//	////var viewEngineResult = _viewEngine.GetView(null, "/Views/Home/Test.cshtml", true);

		//	//if (!viewEngineResult.Success)
		//	//	viewEngineResult = _viewEngine.FindView(actionContext, viewPath, false);

		//	//if (!viewEngineResult.Success)
		//	//{
		//	//	throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewPath));
		//	//}

		//	//return viewEngineResult.View;
		//}

		protected IView FindView(string viewPath, ActionContext actionContext)
		{
			//FindViewTest(viewPath, actionContext);

			var viewEngineResult = _viewEngine.GetView("", viewPath, true);

			if (!viewEngineResult.Success)
			{
				throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewPath));
			}

			return viewEngineResult.View;
		}


		public async Task<string> RenderViewAsync(string viewPath)
		{
			var actionContext = GetActionContext();

			var view = FindView(viewPath, actionContext);

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

				await view.RenderAsync(viewContext).ConfigureAwait(false);

				return output.ToString();
			}
		}

		public async Task<string> RenderViewAsync<TModel>(string viewPath, TModel model)
		{
			var actionContext = GetActionContext();

			var view = FindView(viewPath, actionContext);

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

				await view.RenderAsync(viewContext).ConfigureAwait(false);

				return output.ToString();
			}
		}

		public async Task<string> RenderViewAsync(string viewPath, ViewDataDictionary viewData)
		{
			var actionContext = GetActionContext();

			var view = FindView(viewPath, actionContext);

			using (var output = new StringWriter())
			{
				var viewContext = new ViewContext(
					actionContext,
					view,
					viewData,
					new TempDataDictionary(
						actionContext.HttpContext,
						_tempDataProvider),
					output,
					new HtmlHelperOptions());

				await view.RenderAsync(viewContext).ConfigureAwait(false);

				return output.ToString();
			}
		}

		private ActionContext GetActionContext()
		{
			var httpContext = new DefaultHttpContext();
			httpContext.RequestServices = _serviceProvider;
			return new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
		}
	}
}
