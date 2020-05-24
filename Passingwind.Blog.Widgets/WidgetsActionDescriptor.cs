using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Diagnostics;

namespace Passingwind.Blog.Widgets
{
	[DebuggerDisplay("{WidgetId}")]
	public class WidgetsActionDescriptor : ActionDescriptor
	{
		public string WidgetId { get; set; }

		public string ViewEnginePath => "Views/Default";
	}
}
