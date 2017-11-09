using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public class CommentViewModel
    {
        public bool CommentNestingEnabled { get; set; }

        public string Id { get; set; }

        public string PostId { get; set; }

        public string Author { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get { return AvatarHelper.GetSrc(Email); } }

        public string Website { get; set; }

        public string Content { get; set; }

        public string Country { get; set; }

        //public bool IsApproved { get; set; }

        //public bool IsSpam { get; set; }

        //public bool IsDeleted { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public string ParentId { get; set; }

        public string IP { get; set; }

        public IList<CommentViewModel> Replays { get; set; } = new List<CommentViewModel>();

    }

    public class CommentFormViewModel
    {
        [MaxLength(100)]
        [Required]
        public string Author { get; set; }

        [MaxLength(100)]
        //[EmailAddress]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        [MaxLength(200)]
        public string Website { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string PostId { get; set; }

        public string ParentId { get; set; }


    }

    public class LastCommentFormUserInfo
    {
        public string Author { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }
    }

    public static class CommentViewModelExtensions
    {
        public static IList<CommentViewModel> Format(this IList<CommentViewModel> source)
        {
            var tops = source.Where(t => t.ParentId == null || t.ParentId == Guid.Empty.ToString()).ToList();

            foreach (var item in tops)
            {
                item.Replays = Gets(source, item.Id).ToList();
            }

            return tops.ToList();
        }

        private static IList<CommentViewModel> Gets(IList<CommentViewModel> source, string parentId)
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
