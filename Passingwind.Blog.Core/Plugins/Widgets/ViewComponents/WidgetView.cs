using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins.Widgets.ViewComponents
{
	public abstract class WidgetView
	{
		const string DefaultViewPath = "/Views/Default.cshtml";

		public HttpContext HttpContext => WidgetViewContext.HttpContext;

		public WidgetViewContext WidgetViewContext { get; set; }

		public WidgetDescriptor Descriptor => WidgetViewContext.Descriptor;

		public abstract Task<IWidgetViewResult> InvokeAsync();

		protected IWidgetViewResult View()
		{
			return View<object>(DefaultViewPath, null);
		}

		protected IWidgetViewResult View(string viewPath)
		{
			return View<object>(viewPath, null);
		}

		protected IWidgetViewResult View<TModel>(TModel model)
		{
			return View(DefaultViewPath, model);
		}

		protected IWidgetViewResult View<TModel>(string viewPath, TModel model)
		{
			var viewDataDictionary = new ViewDataDictionary<TModel>(
								metadataProvider: new EmptyModelMetadataProvider(),
								modelState: new ModelStateDictionary())
			{
				Model = model
			};

			return new WidgetViewResult()
			{
				ViewPath = viewPath,
				ViewData = viewDataDictionary,
			};
		}

	}
}
