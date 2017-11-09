using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class PostViewModel : BaseAuditedModel
    {

        [Required, MaxLength(500)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        [DataType(DataType.MultilineText)]
        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsDraft { get; set; }

        public bool IsPublished { get { return !IsDraft; } }

        public bool EnableCommented { get; set; } = true;

        public int CommentsCount { get; set; }

        public int ViewsCount { get; set; }

        public bool EnableComment { get; set; } = true;

        public DateTime PublishedTime { get; set; } = DateTime.Now;

        public IList<string> SelectCategories { get; set; } = new List<string>();

        public string[] SelectTags { get; set; } = new string[] { };

        public string[] AllTags { get; set; }
        public IList<CategoryViewModel> AllCategories { get; set; } = new List<CategoryViewModel>();


        public UserBaseViewModel User { get; set; }


        public string TagsString { get; set; }
    }
}
