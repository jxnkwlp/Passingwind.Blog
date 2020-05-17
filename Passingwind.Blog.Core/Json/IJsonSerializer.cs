using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Json
{
	public interface IJsonSerializer
	{
		string Serialize(object source, JsonSerializerOptions options = null);
		T Deserialize<T>(string json, JsonSerializerOptions options = null);
	}

	public class JsonSerializerOptions
	{
		public bool PropertyNameCaseInsensitive { get; set; }
	}
}
