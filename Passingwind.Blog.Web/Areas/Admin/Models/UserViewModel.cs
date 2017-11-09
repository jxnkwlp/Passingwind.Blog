using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class UserBaseViewModel : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string DisplayName { get; set; }

        [DataType(DataType.MultilineText)]
        public string UserDescription { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool Lockouted { get; set; }

    }

    public class UserViewModel : UserBaseViewModel
    {
        public string[] SelectRoles { get; set; } = new string[] { };

        public IList<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

    }


    public class UserProfileViewModel
    {
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        [DataType(DataType.MultilineText)]
        public string UserDescription { get; set; }
    }
}
