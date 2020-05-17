using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Passingwind.Blog.Widgets
{
	/// <summary>
	/// A base class for widget components.
	/// </summary>
	public abstract class WidgetComponent
	{
		private IUrlHelper _url;
		private readonly dynamic _viewBag;
		private WidgetViewContext _widgetViewContext;
		private ICompositeViewEngine _viewEngine;

		public HttpContext HttpContext => ViewContext?.HttpContext;

		public HttpRequest Request => ViewContext?.HttpContext?.Request;

		public IPrincipal User => ViewContext?.HttpContext?.User;

		public ClaimsPrincipal UserClaimsPrincipal => ViewContext?.HttpContext?.User;

		public RouteData RouteData => ViewContext?.RouteData;

		public ModelStateDictionary ModelState => ViewData?.ModelState;

		public IUrlHelper Url
		{
			get
			{
				if (_url == null)
				{
					// May be null in unit-testing scenarios.
					var services = WidgetViewContext.ViewContext?.HttpContext?.RequestServices;
					var factory = services?.GetRequiredService<IUrlHelperFactory>();
					_url = factory?.GetUrlHelper(WidgetViewContext.ViewContext);
				}

				return _url;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_url = value;
			}
		}

		public WidgetViewContext WidgetViewContext
		{
			get
			{
				// This should run only for the ViewComponent unit test scenarios.
				if (_widgetViewContext == null)
				{
					_widgetViewContext = new WidgetViewContext();
				}

				return _widgetViewContext;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_widgetViewContext = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="ViewContext"/>.
		/// </summary>
		public ViewContext ViewContext => WidgetViewContext.ViewContext;

		/// <summary>
		/// Gets the <see cref="ViewDataDictionary"/>.
		/// </summary>
		public ViewDataDictionary ViewData => WidgetViewContext.ViewData;

		/// <summary>
		/// Gets the <see cref="ITempDataDictionary"/>.
		/// </summary>
		public ITempDataDictionary TempData => WidgetViewContext.TempData;

		/// <summary>
		/// Gets or sets the <see cref="ICompositeViewEngine"/>.
		/// </summary>
		public ICompositeViewEngine ViewEngine
		{
			get
			{
				if (_viewEngine == null)
				{
					// May be null in unit-testing scenarios.
					var services = WidgetViewContext.ViewContext?.HttpContext?.RequestServices;
					_viewEngine = services?.GetRequiredService<ICompositeViewEngine>();
				}

				return _viewEngine;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_viewEngine = value;
			}
		}


		public ContentWidgetViewResult Content(string content)
		{
			if (content is null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return new ContentWidgetViewResult(content);
		}

		public ViewWidgetViewResult View<TModel>(string viewName, TModel model)
		{
			var viewData = new ViewDataDictionary<TModel>(ViewData, model);
			return new ViewWidgetViewResult()
			{
				ViewEngine = ViewEngine,
				ViewName = viewName,
				ViewData = viewData,
			};
		}

		public ViewWidgetViewResult View<TModel>(TModel model)
		{
			return View(null, model: model);
		}

		public ViewWidgetViewResult View(string viewName)
		{
			return View(viewName, ViewData.Model);
		}

		public ViewWidgetViewResult View()
		{
			return View(null);
		}
	}
}
