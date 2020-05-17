using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement(Attributes = "asp-name")]
	public class NameTagHelper : TagHelper
	{
		private const string NameAttributeName = "asp-name";

		[HtmlAttributeName(NameAttributeName)]
		public ModelExpression Name { get; set; }

		[ViewContext, HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; }

		private IHtmlGenerator _generator;

		public NameTagHelper(IHtmlGenerator generator)
		{
			this._generator = generator;

		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			if (this.Name != null)
			{
				if (this.Name.Metadata == null)
				{
					throw new ArgumentException(nameof(Name));
				}
				 
				string value = NameAndIdProvider.GetFullHtmlFieldName(ViewContext, this.Name.Name);

				output.Attributes.SetAttribute("name", value);
			}
		}
	}

	[HtmlTargetElement(Attributes = "asp-id")]
	public class IdTagHelper : TagHelper
	{
		private const string IdAttributeName = "asp-id";

		[HtmlAttributeName(IdAttributeName)]
		public ModelExpression Id { get; set; }

		[ViewContext, HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; }

		private IHtmlGenerator _generator;

		public IdTagHelper(IHtmlGenerator generator)
		{
			this._generator = generator;

		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			if (this.Id != null)
			{
				if (this.Id.Metadata == null)
				{
					throw new ArgumentException(nameof(Id));
				}

				string idFieldName = NameAndIdProvider.GetFullHtmlFieldName(ViewContext, this.Id.Name);
				string idFieldValue = NameAndIdProvider.CreateSanitizedId(this.ViewContext, idFieldName, _generator.IdAttributeDotReplacement);

				output.Attributes.SetAttribute("id", idFieldValue);
			}
		}
	}


}
