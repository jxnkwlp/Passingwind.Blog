using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using MimeKit;
using Passingwind.Blog.Data.Settings;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _emailSettings;
		private readonly BasicSettings _basicSettings;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(EmailSettings emailSettings, BasicSettings basicSettings, ILogger<EmailSender> logger)
		{
			_emailSettings = emailSettings;
			_basicSettings = basicSettings;
			_logger = logger;
		}

		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			if (!_emailSettings.Validate())
			{
				_logger.LogWarning("Email settings is empty.");
				return Task.CompletedTask;
			}

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_basicSettings.Title, _emailSettings.Email));
			message.To.Add(new MailboxAddress(email));

			message.Subject = subject;

			message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = htmlMessage,
			};

			using (var client = new SmtpClient())
			{
				client.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl);

				client.Authenticate(_emailSettings.UserName, _emailSettings.Password);

				try
				{
					client.Send(message);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Send email faild.");
				}

				client.Disconnect(true);
			}

			return Task.CompletedTask;
		}
	}
}
