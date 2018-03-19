using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class CommentViewModel : BaseCreationModel
    {
        [Display(Name = "Post")]
        public string PostId { get; set; }


        public PostViewModel Post { get; set; }

        [Required]
        public string ReplyContent { get; set; }

        public string Author { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Content { get; set; }

        public string Country { get; set; }

        public bool IsPublished
        {
            get
            {
                return IsApproved && !IsSpam && !IsDeleted;
            }
        }

        /// <summary>
        ///  已审核
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        ///  垃圾
        /// </summary>
        public bool IsSpam { get; set; }
        /// <summary>
        ///  删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public string ParentId { get; set; }

        public string IP { get; set; }

    }
}
