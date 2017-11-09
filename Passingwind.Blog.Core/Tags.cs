using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
    public class Tags : Entity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public IList<PostTags> Posts { get; set; } = new List<PostTags>();
    }
}