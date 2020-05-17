using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Settings
{
	public class EmailSettings : ISettings
	{
		[EmailAddress]
		public string Email { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string SmtpHost { get; set; }

		public int SmtpPort { get; set; }

		public string DisplayName { get; set; }

		public bool EnableSsl { get; set; }

		public virtual bool Validate()
		{
			return !string.IsNullOrEmpty(Email)
						&& !string.IsNullOrEmpty(UserName)
						&& !string.IsNullOrEmpty(Password)
						&& !string.IsNullOrEmpty(SmtpHost)
						&& !string.IsNullOrEmpty(DisplayName);
		}

	}
}
