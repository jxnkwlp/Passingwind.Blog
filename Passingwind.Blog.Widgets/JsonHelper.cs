using System.Text.Json;

namespace Passingwind.Blog.Widgets
{
	internal class JsonHelper
	{
		public static JsonSerializerOptions Options => new JsonSerializerOptions()
		{
			AllowTrailingCommas = true,
			IgnoreNullValues = true,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};
	}
}
