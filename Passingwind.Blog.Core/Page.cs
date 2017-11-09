using System;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
    public class Page : AuditedEntity
    {
        [Required, MaxLength(500)]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }

        public string Content { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

        public bool IsFrontPage { get; set; }

        public bool IsShowInList { get; set; }

        public bool Published { get; set; } = true;

        public int DisplayOrder { get; set; }



        //[Obsolete]
        //public DateTime CreatedOn { get; set; }

        //[Obsolete]
        //public DateTime? ModifiedOn { get; set; }

        //public DateTime PublishedTime { get; set; }

        public Page()
        {
        }
    }
}