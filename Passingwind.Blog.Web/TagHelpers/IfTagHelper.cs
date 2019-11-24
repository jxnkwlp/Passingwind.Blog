using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement(Attributes = "asp-condition")]
	public class IfTagHelper : TagHelper
	{
		public override int Order => -1000;

		/// <summary>
		///  条件
		/// </summary>
		[HtmlAttributeName("asp-condition")]
		public bool Condition { get; set; }

		public IfTagHelper()
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
