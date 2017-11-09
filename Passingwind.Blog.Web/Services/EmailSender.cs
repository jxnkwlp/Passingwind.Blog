using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Passingwind.Blog.Web.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(EmailSettings emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject))
                return;

            if (string.IsNullOrEmpty(_emailSettings.Email) || string.IsNullOrEmpty(_emailSettings.SmtpHost))
                return;

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.Email, _emailSettings.DisplayName));
            mimeMessage.To.Add(new MailboxAddress(email));
            mimeMessage.Subject = subject;

            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message, 
            };

            using (var client = new SmtpClient())
            {
                client.Connected += (sender, e) =>
                {
                    _logger.LogInformation("Smtp client connected.");
                };

                client.Disconnected += (sender, e) =>
                {
                    _logger.LogInformation("Smtp client disconnected.");
                };

                client.MessageSent += (sender, e) =>
                {
                    _logger.LogInformation("Smtp client send message.");
                    _logger.LogInformation("Smtp server response: " + e.Response);
                };

                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(true);
            }
        }

    }
}
