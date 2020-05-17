//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System.Net;

//namespace Passingwind.Blog.Web.WebApi
//{
//	public class HttpApiResponseFilter : IActionFilter, IOrderedFilter
//	{
//		public int Order => -10;

//		public void OnActionExecuted(ActionExecutedContext context)
//		{
//			var httpContext = context.HttpContext;
//			if (httpContext.Response != null && (httpContext.Response.StatusCode >= 401 && httpContext.Response.StatusCode <= 503))
//			{
//				var problem = new ProblemDetails()
//				{
//					Status = httpContext.Response.StatusCode,
//					Title = ((HttpStatusCode)httpContext.Response.StatusCode).ToString()
//				};

//				context.Result = new ObjectResult(problem)
//				{
//					StatusCode = httpContext.Response.StatusCode,
//				};

//				context.ExceptionHandled = true;
//			}
//		}

//		public void OnActionExecuting(ActionExecutingContext context)
//		{
//		}

//		private void HandleResponseStatusCode(HttpContext httpContext)
//		{

//		}
//	}
//}
