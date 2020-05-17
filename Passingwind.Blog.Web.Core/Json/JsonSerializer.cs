using Passingwind.Blog.Json;

namespace Passingwind.Blog.Web.Json
{
	public class JsonSerializer : IJsonSerializer
	{
		private readonly JsonSerializerOptions _default = new JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = true,
		};

		public T Deserialize<T>(string json, JsonSerializerOptions options = null)
		{
			options = options ?? _default;

			return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive,
			});
		}

		public string Serialize(object source, JsonSerializerOptions options = null)
		{
			return System.Text.Json.JsonSerializer.Serialize(source);
		}
	}
}
