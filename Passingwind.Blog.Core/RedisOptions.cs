using System.Text;

namespace Passingwind.Blog
{
	public class RedisOptions
	{
		public bool Enabled { get; set; }
		public string Server { get; set; }
		public int Port { get; set; }
		public int Database { get; set; }
		public string Password { get; set; }
		public string Prefix { get; set; }

		public string ToConnectionString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{Server}.{Port}");

			if (Database >= 0)
				sb.Append($",defaultDatabase={Database}");

			if (!string.IsNullOrWhiteSpace(Prefix))
				sb.Append($",password={Password}");

			if (!string.IsNullOrWhiteSpace(Prefix))
				sb.Append($",prefix={Prefix}");

			return sb.ToString();
		}
	}
}
