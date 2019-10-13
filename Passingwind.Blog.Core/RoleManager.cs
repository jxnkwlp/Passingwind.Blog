using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Blog
{
	public class RoleManager : Microsoft.AspNetCore.Identity.RoleManager<Role>
    {
		public RoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
		{
		}

		public IQueryable<Role> GetQueryable()
        {
            return this.Roles;
        }
    }
}