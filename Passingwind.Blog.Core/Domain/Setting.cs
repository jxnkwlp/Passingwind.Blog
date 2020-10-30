namespace Passingwind.Blog.Data.Domains
{
	/// <summary>
	///  Define an Setting
	/// </summary>
	public class Setting : Entity
	{
		public string UserId { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }

		public Setting()
		{

		}

		public Setting(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public Setting(string userId, string key, string value)
		{
			UserId = userId;
			Key = key;
			Value = value;
		}
	}
}