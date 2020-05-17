using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Passingwind.Blog.Web.WebApi
{
	public class HttpApiResponseExceptionFilter : IActionFilter, IOrderedFilter
	{
		public int Order { get; set; } = int.MaxValue - 10;

		public void OnActionExecuting(ActionExecutingContext context) { }

		public void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.Exception != null)
			{
				var exceptionHandler = context.HttpContext.Features.Get<IExceptionHandlerFeature>();

				var problem = new ProblemDetails()
				{
					Status = 500,
					Title = context.Exception.Message,
#if DEBUG
					Detail = context.Exception.ToString(),
#endif
				};

				context.Result = new ObjectResult(problem)
				{
					StatusCode = 500,
				};

				context.ExceptionHandled = true;
			}
		}
	}
}
