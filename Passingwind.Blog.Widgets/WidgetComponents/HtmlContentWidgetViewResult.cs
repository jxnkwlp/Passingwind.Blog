using Microsoft.AspNetCore.Html;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public class HtmlContentWidgetViewResult : IWidgetComponentViewResult
	{
		public IHtmlContent HtmlContent { get; }

		public HtmlContentWidgetViewResult(IHtmlContent htmlContent)
		{
			HtmlContent = htmlContent;
		}

		public Task ExecuteAsync(WidgetViewContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			context.Writer.Write(HtmlContent);

			return Task.CompletedTask;
		}
	}
}
