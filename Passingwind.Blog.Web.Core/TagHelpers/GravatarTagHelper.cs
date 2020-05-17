using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Cryptography;
using System.Text;

namespace Passingwind.Blog.Web.TagHelpers
{
	[HtmlTargetElement("gravatar")]
	[HtmlTargetElement("img", Attributes = "gravatar")]
	public class GravatarTagHelper : TagHelper
	{
		private const string DefaultHash = "00000000000000000000000000000000";
		private const string GravatarUrl = "https://www.gravatar.com/avatar/{0}?d={1}";

		public string Email { get; set; }

		//public GravatarDefault Default { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (!output.Attributes.ContainsName("src"))
			{
				output.Attributes.Add("src", GetAvatarSrc(Email));
			}
		}

		public static string GetAvatarSrc(string email, string defaultImage = "mm")
		{
			if (string.IsNullOrWhiteSpace(email))
				return string.Format(GravatarUrl, DefaultHash, defaultImage);

			var hash = GetHash(email.Trim().ToLowerInvariant());

			return string.Format(GravatarUrl, hash, defaultImage);
		}

		private static string GetHash(string value)
		{
			MD5 algorithm = MD5.Create();
			byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
			string md5 = "";
			for (int i = 0; i < data.Length; i++)
			{
				md5 += data[i].ToString("x2").ToUpperInvariant();
			}
			return md5;
		}
	}

	//public enum GravatarDefault { }
}
