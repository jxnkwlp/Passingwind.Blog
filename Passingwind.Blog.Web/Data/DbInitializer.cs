using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Web;
using System;

namespace Passingwind.Blog.Data
{
    public class DbInitializer : ISingletonService
    {
        private readonly DbContext _dbContext;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public DbInitializer(DbContext context, UserManager userManager, RoleManager roleManager)
        {
            this._dbContext = context;
            this._userManager = userManager;
            this._roleManager = roleManager;

        }

        public void Initialize()
        {
            var result = _dbContext.Database.EnsureCreated();

            if (result)
            {
                var role = new Role()
                {
                    Name = "Administrator",
                };
                _roleManager.CreateAsync(role).Wait();

                _roleManager.CreateAsync(new Role() { Name = "Editor" }).Wait();
                _roleManager.CreateAsync(new Role() { Name = "Anonymous" }).Wait();

                var user = new User()
                {
                    UserName = "admin",
                    Email = "admin@wuliping.cn",
                    DisplayName = "admin",

                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                _userManager.CreateAsync(user, "123456").Wait();

                _userManager.AddToRolesAsync(user, new string[] { role.Name }).Wait();

            }

        }
    }
}
