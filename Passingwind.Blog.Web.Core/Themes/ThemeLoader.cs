using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Passingwind.Blog.Web.Themes
{
	public static class ThemeLoader
	{
		public static IReadOnlyCollection<ThemeDescriptor> Load(string path)
		{
			var result = new List<ThemeDescriptor>();

			if (!Directory.Exists(path))
				return result;

			var root = new DirectoryInfo(path);

			foreach (var item in root.GetDirectories())
			{
				if (!File.Exists(Path.Combine(item.FullName, "description.json")))
					continue;

				var description = GetDescription(item.FullName);
				description.Id = item.Name;

				var descriptor = new ThemeDescriptor()
				{
					Description = description,
					Path = item.FullName,
					Name = item.Name,
				};

				result.Add(descriptor);
			}

			return result;
		}

		private static ThemeDescription GetDescription(string path)
		{
			var file = Path.Combine(path, "description.json");

			string json = File.ReadAllText(file);

			return JsonSerializer.Deserialize<ThemeDescription>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, });
		}
	}
}
