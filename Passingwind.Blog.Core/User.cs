using Microsoft.AspNetCore.Identity;
using System;

namespace Passingwind.Blog
{
    /// <summary>
    /// user entity class
    /// </summary>
    public class User : IdentityUser<string>
    {
        public const string DefaultAdministratorUserName = "Admin";

        public string DisplayName { get; set; }

        public string UserDescription { get; set; }

        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return this.Email;
            return this.DisplayName;
        }

        public bool IsLockouted
        {
            get
            {
                if (LockoutEnabled && LockoutEnd.HasValue)
                {
                    var endTime = LockoutEnd.Value.UtcDateTime;

                    return DateTimeOffset.UtcNow < endTime;
                }
                return false;
            }
        }


        public User()
        {

        }
    }
}