using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    public class UserManager : Microsoft.AspNetCore.Identity.UserManager<User>
    {
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            var verifyResult = this.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return await Task.FromResult(verifyResult == PasswordVerificationResult.Success);
        }

        public async Task RemoveFromAllRolesAsync(User user)
        {
            var allRoles = await this.GetRolesAsync(user);
            if (allRoles != null)
            {
                await this.RemoveFromRolesAsync(user, allRoles);
            }
        }

        public IQueryable<User> GetQueryable()
        {
            return this.Users;
        }
    }
}