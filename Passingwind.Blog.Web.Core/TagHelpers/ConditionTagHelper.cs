using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement(Attributes = "asp-condition")]
	public class ConditionTagHelper : TagHelper
	{
		/// <summary>
		///  条件
		/// </summary>
		[HtmlAttributeName("asp-condition")]
		public bool Condition { get; set; }

		public ConditionTagHelper()
		{
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}

			if (!Condition)
			{
				output.SuppressOutput();
			}
		}
	}
}
