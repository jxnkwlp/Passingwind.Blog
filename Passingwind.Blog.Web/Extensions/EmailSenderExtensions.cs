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
        ///  ���������ʼ�
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task SendCommentEmailAsync(this IEmailSender emailSender, EmailSettings emailSettings, Post post, string commentLink, Comment comment, Comment parentComment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            // ���͸�����Ա��ʾ

            string content = $"���¡�{post.Title}�����������ˣ�<a href='{HtmlEncoder.Default.Encode(commentLink)}'>���</a>�鿴��";
            await emailSender.SendEmailAsync(emailSettings.Email, $"���¡�{post.Title}����������", content);

            // ���͸� ���ظ���
            if (!string.IsNullOrEmpty(comment.ParentId) && parentComment != null && !string.IsNullOrEmpty(parentComment.Email))
            {
                string content2 = $"������ۡ�{comment.Content}�������»ظ���<a href='{HtmlEncoder.Default.Encode(commentLink)}'>���</a>�鿴��";

                await emailSender.SendEmailAsync(parentComment.Email, $"���¡�{post.Title}����������", content);
            }
        }
    }
}
