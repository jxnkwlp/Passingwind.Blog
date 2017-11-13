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
            string content = $@"���¡�{post.Title}�����������ˣ�<a href='{HtmlEncoder.Default.Encode(commentLink)}'>���</a>�鿴��<br/>
    <table>
        <tr>
            <th>Name</th>
            <td>{comment.Author}</td> 
        </tr>
        <tr>
            <th>Email</th>
            <td>{comment.Email}</td> 
        </tr>
        <tr>
            <th>Website</th>
            <td>{comment.Website}</td> 
        </tr>
        <tr>
            <th>Content</th>
            <td>{comment.Content}</td>
        </tr>
    </table>
";

            await emailSender.SendEmailAsync(emailSettings.Email, $"���¡�{post.Title}����������", content);

            // ���͸� ���ظ���
            if (!string.IsNullOrEmpty(comment.ParentId) && parentComment != null && !string.IsNullOrEmpty(parentComment.Email))
            {
                string content2 = $"������ۡ�{parentComment.Content}�������»ظ���{comment.Content}����<a href='{HtmlEncoder.Default.Encode(commentLink)}'>���</a>�鿴Դ��վ��";

                await emailSender.SendEmailAsync(parentComment.Email, $"���¡�{post.Title}�����ۻظ�����", content);
            }
        }
    }
}
