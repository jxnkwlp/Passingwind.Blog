using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Passingwind.Blog
{
	public static class AvatarHelper
	{
		private const string defaultHash = "00000000000000000000000000000000";
		private const string gravatarUrl = "https://www.gravatar.com/avatar/{0}?d={1}";

		public static string GetSrc(string email, string defaultImage = "mm")
		{
			if (string.IsNullOrWhiteSpace(email))
				return string.Format(gravatarUrl, defaultHash, defaultImage);

			var hash = GetMd5String(email.Trim().ToLowerInvariant());

			return string.Format(gravatarUrl, hash, defaultImage);
		}

		private static string GetMd5String(string source)
		{
			var md5 = MD5.Create();

			var data = md5.ComputeHash(Encoding.UTF8.GetBytes(source));

			var result = string.Concat(data.Select(t => t.ToString("x2").ToLowerInvariant()));

			return result;
		}
	}
}
