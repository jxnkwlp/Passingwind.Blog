using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class PageViewModel : BaseAuditedModel
    {
        [Required, MaxLength(500)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool Published { get; set; } = true;

        public int DisplayOrder { get; set; } = 1;

        public string Keywords { get; set; }

    }
}
