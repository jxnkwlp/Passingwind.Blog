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
			var nlogger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

			nlogger.Info("Application start...");

			try
			{
				var host = CreateHostBuilder(args).Build();

				using (var scope = host.Services.CreateScope())
				{
					var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
					var dbConnectionString = scope.ServiceProvider.GetRequiredService<DbConnectionString>();

					logger.LogInformation($"Database provider: {dbConnectionString.Provider}");

					// 
					new BlogDataSender(scope.ServiceProvider).InitialAsync().Wait();
				}

				host.Run();
			}
			catch (Exception ex)
			{
				nlogger.Error(ex, "Stopped program because of exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureLogging(logging =>
				{
					//logging.ClearProviders();
				})
				.UseNLog();
	}
}
