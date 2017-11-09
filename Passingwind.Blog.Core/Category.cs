using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
    /// <summary>
    ///
    /// </summary>
    public class Category : Entity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

        [Required]
        public string Slug { get; set; }

        public int DisplayOrder { get; set; }

        public IList<PostCategory> Posts { get; set; } = new List<PostCategory>();
    }
}