using Passingwind.Blog.Data.Domains;

namespace Passingwind.Blog.Web.Models
{
	public class CommentEmailModel
	{
		public Comment SourceComment { get; set; }
		public Comment Replay { get; set; }

		public string CommentUrl { get; set; }

		public bool IsPostAuthorEmail { get; set; }

		public Post Post { get; set; }
	}
}
