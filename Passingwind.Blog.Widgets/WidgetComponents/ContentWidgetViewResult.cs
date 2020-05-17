using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widgets
{
	public class ContentWidgetViewResult : IWidgetComponentViewResult
	{
		public string Content { get; }

		public ContentWidgetViewResult(string content)
		{
			Content = content;
		}
		 
		public Task ExecuteAsync(WidgetViewContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			context.HtmlEncoder.Encode(context.Writer, Content);

			return Task.CompletedTask;
		}
	}
}
