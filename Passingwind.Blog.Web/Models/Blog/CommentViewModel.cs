using Passingwind.Blog.Data.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Passingwind.Blog.Web.Models
{
	public class CommentViewModel
	{
		public bool CommentNestingEnabled { get; set; }

		public int Id { get; set; }

		public int PostId { get; set; }

		public string Author { get; set; }

		public string Email { get; set; }

		public string AvatarUrl { get { return AvatarHelper.GetSrc(Email); } }

		public string Website { get; set; }

		public string Content { get; set; }

		public string Country { get; set; }

		public DateTime CreationTime { get; set; } = DateTime.Now;

		public int? ParentId { get; set; }

		public string IP { get; set; }

		public IList<CommentViewModel> Replays { get; set; } = new List<CommentViewModel>();

	}

	public class CommentFormViewModel
	{
		[MaxLength(32)]
		[Required]
		public string Author { get; set; }

		[MaxLength(64)]
		[EmailAddress]
		[Required]
		public string Email { get; set; }

		[DataType(DataType.Url)]
		[MaxLength(128)]
		public string Website { get; set; }

		[Required]
		public string Content { get; set; }

		[Required]
		public int PostId { get; set; }

		public int? ParentId { get; set; }

		public string CaptchaCode { get; set; }
		public string CaptchaId { get; set; }

		public bool EnableCaptchaCode { get; set; }
	}

	public class LastCommentFormUserInfo
	{
		public string Author { get; set; }

		public string Email { get; set; }

		public string Website { get; set; }
	}

	public static class CommentViewModelExtensions
	{
		public static CommentViewModel ToViewModel(this Comment comment, bool commentNestingEnabled)
		{
			return new CommentViewModel()
			{
				Author = comment.Author,
				CommentNestingEnabled = commentNestingEnabled,
				Content = comment.Content,
				Country = comment.Country,
				CreationTime = comment.CreationTime,
				Email = comment.Email,
				Id = comment.Id,
				IP = comment.IP,
				ParentId = comment.ParentId,
				PostId = comment.PostId,
				Website = comment.Website,
			};
		}

		public static IList<CommentViewModel> Format(this IList<CommentViewModel> source)
		{
			var tops = source.Where(t => t.ParentId == null || t.ParentId == 0).ToList();

			foreach (var item in tops)
			{
				item.Replays = Gets(source, item.Id).ToList();
			}

			return tops.ToList();
		}

		private static IList<CommentViewModel> Gets(IList<CommentViewModel> source, int parentId)
		{
			var result = source.Where(t => t.ParentId == parentId).ToList();

			foreach (var item in result)
			{
				item.Replays = Gets(source, item.Id).ToList();
			}

			return result;
		}
	}
}
