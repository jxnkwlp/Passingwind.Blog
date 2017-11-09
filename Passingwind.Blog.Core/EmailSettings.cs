using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
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
    }
}