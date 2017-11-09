using Passingwind.Blog.Web.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;

namespace Passingwind.Blog.Web
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        /// <summary>
        ///  发送评论邮件
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task SendCommentEmailAsync(this IEmailSender emailSender, EmailSettings emailSettings, Post post, string commentLink, Comment comment, Comment parentComment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            // 发送给管理员提示

            string content = $"文章“{post.Title}”有新评论了，<a href='{HtmlEncoder.Default.Encode(commentLink)}'>点此</a>查看。";
            await emailSender.SendEmailAsync(emailSettings.Email, $"文章“{post.Title}”评论提醒", content);

            // 发送给 被回复者
            if (!string.IsNullOrEmpty(comment.ParentId) && parentComment != null && !string.IsNullOrEmpty(parentComment.Email))
            {
                string content2 = $"你的评论“{comment.Content}”有了新回复，<a href='{HtmlEncoder.Default.Encode(commentLink)}'>点此</a>查看。";

                await emailSender.SendEmailAsync(parentComment.Email, $"文章“{post.Title}”评论提醒", content);
            }
        }
    }
}
