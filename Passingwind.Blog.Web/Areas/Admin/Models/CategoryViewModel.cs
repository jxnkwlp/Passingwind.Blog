using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class CategoryViewModel : BaseModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        public string Slug { get; set; }

        public int DisplayOrder { get; set; } = 1;


        public int Count { get; set; }
    }
}
