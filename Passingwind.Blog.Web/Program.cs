using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Passingwind.Blog.Data;
using System;

namespace Passingwind.Blog.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			NLog.Logger nlogger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

			nlogger.Info("Application start...");

			try
			{
				var builder = CreateHostBuilder(args).Build();
				using (var scope = builder.Services.CreateScope())
				{
					var logger = scope.ServiceProvider.GetService<ILogger<DbInitializer>>();
					var db = scope.ServiceProvider.GetService<BlogDbContext>();
					var userManager = scope.ServiceProvider.GetService<UserManager>();
					var roleManager = scope.ServiceProvider.GetService<RoleManager>();
					var postManager = scope.ServiceProvider.GetService<PostManager>();

					new DbInitializer(db, userManager, roleManager, postManager).Initialize();
				}

				builder.Run();
			}
			catch (Exception ex)
			{
				nlogger.Error(ex);
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}

		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host
			.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			})
			//.ConfigureLogging(logging =>
			//{
			//	logging.ClearProviders();
			//	logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
			//})
			.UseNLog();
	}
}
