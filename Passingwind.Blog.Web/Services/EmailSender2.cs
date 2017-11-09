using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender2 : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender2(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName);
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                mailMessage.BodyEncoding = Encoding.UTF8;

                client.EnableSsl = _emailSettings.EnableSsl;
                client.Host = _emailSettings.SmtpHost;
                client.Port = _emailSettings.SmtpPort;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.UseDefaultCredentials = true;

                client.Send(mailMessage);

            }

            return Task.CompletedTask;
        }
    }
}
