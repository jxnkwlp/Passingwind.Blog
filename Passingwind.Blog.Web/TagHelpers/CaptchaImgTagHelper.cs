using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement("captcha-img")]
	public class CaptchaImgTagHelper : TagHelper
	{
		protected IUrlHelperFactory UrlHelperFactory;

		public CaptchaImgTagHelper(IUrlHelperFactory UrlHelperFactory)
		{
			this.UrlHelperFactory = UrlHelperFactory;
		}

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public string Url { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			base.Process(context, output);

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			string newId = Guid.NewGuid().ToString("N");

			TagBuilder builder = new TagBuilder("img");
			builder.Attributes["src"] = GetUrl(newId);
			builder.Attributes["id"] = "captcha_" + newId;
			builder.Attributes["onclick"] = $"_chageCaptcha_{newId}()";

			builder.MergeAttributes(context.AllAttributes.ToDictionary(t => t.Name, t => t.Value));

			var idTag = GenerateGuidTag(newId);

			//output.SuppressOutput();
			output.TagName = null;

			output.Content.AppendHtml(builder.RenderSelfClosingTag());
			output.Content.AppendHtml(idTag.RenderSelfClosingTag());

			output.PostContent.AppendHtml(GetCaptchaScripts(newId));
		}

		private TagBuilder GenerateGuidTag(string guid)
		{
			TagBuilder builder = new TagBuilder("input");
			builder.Attributes["type"] = "hidden";
			builder.Attributes["name"] = "captchaId";
			builder.Attributes["value"] = guid;

			return builder;
		}

		private string GetUrl(string id)
		{
			return Url ?? UrlHelperFactory.GetUrlHelper(ViewContext).CaptchaImageLink(id);
		}

		private string GetCaptchaScripts(string id)
		{
			return $"<script>function _chageCaptcha_{id}(){{document.getElementById('captcha_{id}').src ='{GetUrl(id)}'+'?'+new Date().valueOf()}}</script>";
		}
	}
}
