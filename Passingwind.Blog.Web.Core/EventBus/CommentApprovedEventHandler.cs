using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.EventBus;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.EventBus
{
	public class CommentApprovedEventHandler : IEventBusHandler<CommentApprovedEventData>
	{
		const string emailTemplateName = "/Views/Template/CommentEmail.cshtml";

		private readonly IRazorViewService _razorService;
		private readonly IEmailSender _emailSender;
		private readonly ILogger<CommentApprovedEventHandler> _logger;

		public CommentApprovedEventHandler(IRazorViewService razorService, IEmailSender emailSender, ILogger<CommentApprovedEventHandler> logger)
		{
			_razorService = razorService;
			_emailSender = emailSender;
			_logger = logger;
		}

		public async Task HandleAsync(CommentApprovedEventData data)
		{
			_logger.LogDebug($"{nameof(CommentApprovedEventHandler)}.{nameof(HandleAsync)}");

			// sender to user
			if (data.SourceComment != null)
			{
				var html = await _razorService.RenderViewAsync(emailTemplateName, new CommentEmailModel()
				{
					Replay = data.Replay,
					SourceComment = data.SourceComment,
					CommentUrl = data.CommentUrl,
					IsPostAuthorEmail = false,

				});

				await _emailSender.SendEmailAsync(data.SourceComment.Email, "new comment.", html);
			}

			// sender to post author
			await SendToPostAuthorAsync(data);
		}

		private async Task SendToPostAuthorAsync(CommentApprovedEventData data)
		{
			var html = await _razorService.RenderViewAsync(emailTemplateName, new CommentEmailModel()
			{
				IsPostAuthorEmail = true,
				Replay = data.Replay,
				CommentUrl = data.CommentUrl,
				Post = data.Post
			});

			await _emailSender.SendEmailAsync(data.PostAuthorEmail, "new comment.", html);
		}
	}
}
