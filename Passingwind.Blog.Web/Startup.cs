using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Passingwind.Blog.Data;
using Passingwind.Blog.EntityFramework.MySql;
using Passingwind.Blog.EntityFramework.PostgreSQL;
using Passingwind.Blog.EntityFramework.Sqlite;
using Passingwind.Blog.EntityFramework.SqlServer;
using Passingwind.Blog.Web.Middlewares;
using Passingwind.Blog.Web.Services;
using Passingwind.Blog.Web.Themes;
using Passingwind.Blog.Web.UI.Widgets;
using Passingwind.Blog.Widgets;
using Passingwind.Blog.Widgets.WidgetComponents;
using System;
using System.Text.Unicode;

namespace Passingwind.Blog.Web
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IHostEnvironment HostEnvironment { get; }
		public BlogOptions BlogOptions { get; private set; }

		public DbConnectionString DbConnectionString { get; private set; }

		public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			Configuration = configuration;
			HostEnvironment = hostEnvironment;

			BlogOptions = new BlogOptions();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<BlogOptions>(Configuration.GetSection("Blog"));
			Configuration.GetSection("Blog").Bind(BlogOptions);

			var redisOptions = new RedisOptions();
			services.Configure<RedisOptions>(Configuration.GetSection("Redis"));
			Configuration.GetSection("Redis").Bind(redisOptions);

			DbConnectionString = new DbConnectionString
			{
				Provider = Configuration.GetValue<DbConnectionProvider>("DatabaseProvider"),
				ConnectionString = Configuration.GetConnectionString("Blog")
			};

			services.AddSingleton(DbConnectionString);

			if (DbConnectionString.Provider == DbConnectionProvider.SqlServer)
			{
				services.AddSqlServerDbContext(DbConnectionString);
			}
			else if (DbConnectionString.Provider == DbConnectionProvider.Sqlite)
			{
				services.AddSqliteDbContext(DbConnectionString);
			}
			else if (DbConnectionString.Provider == DbConnectionProvider.Mysql)
			{
				services.AddMySqlDbContext(DbConnectionString);
			}
			else if (DbConnectionString.Provider == DbConnectionProvider.PostgreSQL)
			{
				services.AddPostgreSQLDbContext(DbConnectionString);
			}

			services.AddDbContext<BlogDbContext>((s, options) =>
			{
#if DEBUG
				options.EnableDetailedErrors();
				options.EnableSensitiveDataLogging(true);
#endif
				// cache query
				options.AddInterceptors(s.GetRequiredService<SecondLevelCacheInterceptor>());
			});

			services.TryAddScoped<DbContext, BlogDbContext>();

			services.AddEFSecondLevelCache(options =>
			{
				options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(60));
				if (redisOptions.Enabled)
				{
					options.UseEasyCachingCoreProvider("redis");
				}
				else
					options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(60));
			});

			services.ConfigureEasyCaching(redisOptions);

			services.ConfigureIdenittyService(Configuration, BlogOptions);

			services.ConfigureCoreService();

			services.ConfigureSwaggerGenService();

			services.ConfigureApplicationServices();

			services.ConfigureAutoMapperService();

			services.ConfigureEventBus();

			services.ConfigureMiniProfiler();

			services.AddSingleton<IWidgetConfigurationService, WidgetManager>();
			services.AddSingleton<IWidgetManager, WidgetManager>();
			services.AddWidgets(options =>
			{
				options.ShardTypes = new[] { typeof(Startup), typeof(BlogOptions), typeof(IWidget) };
			});
			services.Replace<IWidgetViewLocationExpander, WidgetViewLocationExpander>(ServiceLifetime.Scoped);

			services.AddThemes(HostEnvironment);

			services.AddSpaStaticFiles(options =>
			{
				options.RootPath = "ClientApp/dist";
			});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

#if DEBUG
			app.UseCors();
#endif

			if (BlogOptions.EnableMiniProfiler)
				app.UseMiniProfiler();

			app.UseImageAxdMiddleware();

			if (Configuration.GetValue("HttpsRedirection", false))
				app.UseHttpsRedirection();

			app.UseResponseCaching();
			app.UseResponseCompression();

			app.UseStaticFiles();
			app.UseSpaStaticFiles(new StaticFileOptions() { RequestPath = new PathString("/admin"), });

			app.UseWidgets();
			app.UseThemeMiddleware();

			//app.UseRequestLocalization();

			app.UseSession();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			if (Configuration.GetValue("Swagger:Enabled", false))
				app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "Default",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
				endpoints.MapDefaultControllerRoute();

#if DEBUG
				//endpoints.MapToVueCliProxy(
				//	"{*path}",
				//	new SpaOptions
				//	{
				//		SourcePath = "ClientApp",
				//		StartupTimeout = TimeSpan.FromSeconds(60)
				//	},
				//	npmScript: "serve",
				//	regex: "Compiled successfully",
				//	forceKill: true);
#endif
			});

			app.MapWhen(c => c.Request.Path.StartsWithSegments("/admin"), builder =>
			{
				builder.UseSpa(c =>
				{
					c.Options.SourcePath = "ClientApp";

#if DEBUG
					// c.UseProxyToSpaDevelopmentServer("http://localhost:8080/admin");
#endif
				});
			});

		}
	}
}
