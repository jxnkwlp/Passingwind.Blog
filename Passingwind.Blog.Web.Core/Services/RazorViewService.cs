using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Passingwind.Blog.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
	public class RazorViewService : IRazorViewService
	{
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IServiceProvider _serviceProvider;

		public RazorViewService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
		{
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_httpContextAccessor = httpContextAccessor;
			_serviceProvider = serviceProvider;
		}

		public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model) where TModel : class
		{
			var actionContext = GetActionContext();

			var view = FindView(viewName, actionContext);

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

		public async Task<string> RenderViewAsync(string viewName)
		{
			var actionContext = GetActionContext();

			var view = FindView(viewName, actionContext);

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

		protected IView FindView(string viewName, ActionContext actionContext)
		{
			var getViewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
			if (getViewResult.Success)
			{
				return getViewResult.View;
			}

			var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: true);
			if (findViewResult.Success)
			{
				return findViewResult.View;
			}

			var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
			var errorMessage = string.Join(
				Environment.NewLine,
				new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

			throw new InvalidOperationException(errorMessage);
		}

		private ActionContext GetActionContext()
		{
			var httpContext = new DefaultHttpContext();
			httpContext.RequestServices = _serviceProvider;
			return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
			//return new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
		}
	}
}
