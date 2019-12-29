using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Passingwind.Blog.BlogML;
using Passingwind.Blog.Data;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Web.Captcha;
using Passingwind.Blog.Web.Services;
using System;
using System.Runtime.Loader;

namespace Passingwind.Blog.Web
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public bool UseHttps { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			UseHttps = configuration.GetValue("UseHttps", false);
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			// If using IIS:
			services.Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			services.AddDbContext<BlogDbContext>(options =>
			{
				var dbType = Configuration.GetValue<string>("DbType");
				if (string.Equals(dbType, "sqlite", StringComparison.OrdinalIgnoreCase))
				{
					options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
				}
				else if (string.Equals(dbType, "sqlserver", StringComparison.OrdinalIgnoreCase))
				{
					options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
				}
				else
				{
					throw new NotSupportedException("Unknow the databse type !");
				}

#if DEBUG
				options.EnableSensitiveDataLogging(true);
				options.EnableDetailedErrors(true);
#endif

			}) // default Scoped ServiceLifetime
			.AddScoped<DbContext, BlogDbContext>()
			// .AddScoped<DbInitializer>()
			.AddScoped<EntityStore>();

			services.AddIdentity<User, Role>(options =>
			{
				// set password rule 
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireDigit = true;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;

			})
			.AddUserManager<UserManager>()
			.AddRoleManager<RoleManager>()
			.AddSignInManager<SignInManager>()
			.AddEntityFrameworkStores<DbContext>()
			.AddDefaultTokenProviders();

			// Add application services.
			services.AddTransient<PostManager>();
			services.AddTransient<PageManager>();
			services.AddTransient<CommentManager>();
			services.AddTransient<CategoryManager>();
			services.AddTransient<TagsManager>();
			services.AddTransient<SettingManager>();
			services.AddTransient<IEmailSender, EmailSender>();
			//services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddTransient<BlogMLImporter>();
			services.AddTransient<BlogMLExporter>();
			services.AddTransient<ICaptchaService, CaptchaService>();
			services.AddTransient<IFileService, LocalFileService>();
			services.AddTransient<IPluginDataStore, PluginDataStore>();

			services.AddSingleton<IApplicationRestart, ApplicationRestart>();

			services.AddMemoryCache();
			services.AddSession(options =>
			{
				// options.IdleTimeout = TimeSpan.FromSeconds(60);
				options.Cookie.IsEssential = true;
				options.Cookie.HttpOnly = true;
			});

			services.Configure<RazorViewEngineOptions>(options =>
			{
				//options.ViewLocationFormats.Add("~/Plugins/{1}/{0}.cshtml");
				//options.ViewLocationFormats.Add("~/Plugins/{2}/views/{1}/{0}.cshtml");
				//options.ViewLocationExpanders.Add(new WidgetViewLocationExpander());
			});

			services.AddRazorPages();
			services.AddControllersWithViews()
				//.AddViewOptions(o => o.ViewEngines.Add(new WidgetViewEngine()))
				.AddPlugins();

			services.AddResponseCaching();
			services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
			services.AddResponseCompression(options =>
			{
				options.EnableForHttps = false;
			});

			services.AddAuthorization(options =>
			{
				// options.AddPolicy("Admin", p => p.AddRequirements(new AdminRequirement()));
			});

			services.AddScoped(s => s.GetService<SettingManager>().LoadSetting<BasicSettings>());
			services.AddScoped(s => s.GetService<SettingManager>().LoadSetting<AdvancedSettings>());
			services.AddScoped(s => s.GetService<SettingManager>().LoadSetting<EmailSettings>());
			services.AddScoped(s => s.GetService<SettingManager>().LoadSetting<CommentsSettings>());
			services.AddScoped(s => s.GetService<SettingManager>().LoadSetting<FeedSettings>());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseStatusCodePages();
			}
			else
			{
				//app.UseStatusCodePagesWithRedirects("~/error/{0}");
				app.UseExceptionHandler("/error");

				app.UseHsts();
			}

			app.UseResponseCompression();
			app.UseResponseCaching();

			if (UseHttps)
				app.UseHttpsRedirection();

			app.UseStaticFiles(new StaticFileOptions()
			{
				OnPrepareResponse = context =>
				{
					// Cache static file for 1 year
					if (!string.IsNullOrEmpty(context.Context.Request.Query["v"]))
					{
						context.Context.Response.Headers.Add("cache-control", new[] { "public, max-age=31536000" });
						context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123
					}
				}
			});

			app.UseImageAxdMiddleware();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseCookiePolicy();

			app.UseSession();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				RegisterControllerRoutes(endpoints);
			});
		}

		private void RegisterControllerRoutes(IEndpointRouteBuilder endpoint)
		{
			endpoint.MapAreaControllerRoute(
					name: "admin",
					areaName: "admin",
					pattern: "admin/{controller=Default}/{action=Index}/{id?}");

			endpoint.MapControllerRoute(
				name: RouteNames.LogIn,
				pattern: "account/login",
				defaults: new { controller = "account", action = "login" });
			endpoint.MapControllerRoute(
				name: RouteNames.Logout,
				pattern: "account/logout",
				defaults: new { controller = "account", action = "logout" });
			endpoint.MapControllerRoute(
				name: RouteNames.Register,
				pattern: "account/register",
				defaults: new { controller = "account", action = "register" });
			endpoint.MapControllerRoute(
				name: RouteNames.ChangePassword,
				pattern: "account/changePassword",
				defaults: new { controller = "account", action = "changepassword" });


			endpoint.MapControllerRoute(
				name: RouteNames.Home,
				pattern: "",
				defaults: new { controller = "Home", action = "Index" });

			endpoint.MapControllerRoute(
			   name: RouteNames.Archive,
			   pattern: "archive",
			   defaults: new { controller = "home", action = "archive" });

			endpoint.MapControllerRoute(
				name: RouteNames.Post,
				pattern: "post/{slug}",
				defaults: new { controller = "home", action = "post" });

			endpoint.MapControllerRoute(
				name: RouteNames.Page,
				pattern: "page/{slug}",
				defaults: new { controller = "home", action = "page" });

			endpoint.MapControllerRoute(
			  name: RouteNames.Author,
			  pattern: "author/{username}",
			  defaults: new { controller = "home", action = "index" });

			endpoint.MapControllerRoute(
			   name: RouteNames.Tags,
			   pattern: "tag/{name}",
			   defaults: new { controller = "home", action = "index" });

			endpoint.MapControllerRoute(
			   name: RouteNames.Category,
			   pattern: "category/{name}",
			   defaults: new { controller = "home", action = "index" });

			endpoint.MapControllerRoute(
			   name: RouteNames.Monthlist,
			   pattern: "{year:int}/{month:range(1,12)}",
			   defaults: new { controller = "home", action = "index" });

			endpoint.MapControllerRoute(
				name: RouteNames.Search,
				pattern: "search",
				defaults: new { controller = "home", action = "index" });

			endpoint.MapControllerRoute(
				name: RouteNames.CommentForm,
				pattern: "comment/{postId}",
				defaults: new { controller = "home", action = "AddComment" });

			endpoint.MapControllerRoute(
				name: RouteNames.NotFound,
				pattern: "notfound",
				defaults: new { controller = "home", action = "notfound" });

			// default  
			endpoint.MapControllerRoute(
					name: "areas",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

			endpoint.MapDefaultControllerRoute();
		}

	}
}
