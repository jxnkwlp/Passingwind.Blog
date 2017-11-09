using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.TagHelpers
{
    [HtmlTargetElement("if")]
    [HtmlTargetElement("asp-condition")]
    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        /// <summary>
        ///  条件
        /// </summary>
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

            if (Condition)
            {
                output.SuppressOutput();
            }
            else
            {
                return;
            }
        }
    }
}
