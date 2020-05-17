using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Data
{
	public class BlogDataSender
	{
		private readonly IServiceProvider _serviceProvider;

		public BlogDataSender(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task InitialAsync()
		{
			var logger = _serviceProvider.GetRequiredService<ILogger<BlogDataSender>>();
			var dbContext = _serviceProvider.GetRequiredService<BlogDbContext>();
			var userManager = _serviceProvider.GetRequiredService<BlogUserManager>();
			var roleManager = _serviceProvider.GetRequiredService<BlogRoleManager>();

			logger.LogInformation("Start Database migrate...");

			// create and apply migrations 
			dbContext.Database.Migrate();

			var role1 = new Role()
			{
				Name = Role.AdministratorName,
			};
			var role2 = new Role()
			{
				Name = Role.EditorName,
			};
			var role3 = new Role()
			{
				Name = Role.Anonymous,
			};


			if (!dbContext.Roles.Any())
			{
				await roleManager.CreateAsync(role1);
				await roleManager.CreateAsync(role2);
				await roleManager.CreateAsync(role3);
			}

			if (!dbContext.Users.Any())
			{
				var user1 = new User()
				{
					UserName = $"{User.DefaultAdministratorUserName.ToLower()}@blog.com",
					Email = $"{User.DefaultAdministratorUserName.ToLower()}@blog.com",
					DisplayName = User.DefaultAdministratorUserName,
					EmailConfirmed = true,
					CreationTime = DateTime.Now,
					TwoFactorEnabled = false,
					LockoutEnabled = false,
				};
				user1.UserRoles.Add(new UserRole() { Role = role1 });

				await userManager.CreateAsync(user1, "123qweasd");
			}

			logger.LogInformation("End Database migrate .");
		}
	}
}
