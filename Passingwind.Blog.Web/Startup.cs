using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.Web.Models;
using Passingwind.Blog.Web.Services;
using Microsoft.AspNetCore.Routing;
using Passingwind.Blog.Data;
using Passingwind.Blog.BlogML;
using System.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;

namespace Passingwind.Blog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddAutoRegisterService();

            services.AddDbContextPool<BlogDbContext>(options =>
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
            services.AddTransient<IFileService, LocalFileService>();

            services.AddScoped<DbInitializer>();


            services.AddMemoryCache();
            services.AddSession();

            services.AddMvc();

            // services.AddResponseCaching();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbInitializer dbInitializer)
        {
            // init database 
            dbInitializer.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            //app.UseStatusCodePagesWithRedirects("~/error/{0}");

            app.UseResponseCompression();

            // app.UseResponseCaching();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    // Cache static file for 1 year
                    if (!string.IsNullOrEmpty(context.Context.Request.Query["v"]))
                    {
                        context.Context.Response.Headers.Add("cache-control", new[] { "public,max-age=31536000" });
                        context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123
                    }
                }
            });

            app.UseImageAxdMiddleware();

            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(RegisterRoutes);
        }

        private void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapAreaRoute(
                    name: "admin",
                    areaName: "admin",
                    template: "admin/{controller=Default}/{action=Index}/{id?}");

            routes.MapRoute(
                name: RouteNames.LogIn,
                template: "account/login",
                defaults: new { controller = "account", action = "login" });
            routes.MapRoute(
                name: RouteNames.Logout,
                template: "account/logout",
                defaults: new { controller = "account", action = "logout" });
            routes.MapRoute(
                name: RouteNames.Register,
                template: "account/register",
                defaults: new { controller = "account", action = "register" });
            routes.MapRoute(
                name: RouteNames.ChangePassword,
                template: "account/changePassword",
                defaults: new { controller = "account", action = "changepassword" });


            routes.MapRoute(
                name: RouteNames.Home,
                template: "",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
               name: RouteNames.Archive,
               template: "archive",
               defaults: new { controller = "home", action = "archive" });

            routes.MapRoute(
                name: RouteNames.Post,
                template: "post/{slug}",
                defaults: new { controller = "home", action = "post" });

            routes.MapRoute(
                name: RouteNames.Page,
                template: "page/{slug}",
                defaults: new { controller = "home", action = "page" });

            routes.MapRoute(
              name: RouteNames.Author,
              template: "author/{username}",
              defaults: new { controller = "home", action = "index" });

            routes.MapRoute(
               name: RouteNames.Tags,
               template: "tag/{name}",
               defaults: new { controller = "home", action = "index" });

            routes.MapRoute(
               name: RouteNames.Category,
               template: "category/{name}",
               defaults: new { controller = "home", action = "index" });

            routes.MapRoute(
               name: RouteNames.Monthlist,
               template: "{year:int}/{month:range(1,12)}",
               defaults: new { controller = "home", action = "index" });

            routes.MapRoute(
                name: RouteNames.Search,
                template: "search",
                defaults: new { controller = "home", action = "index" });

            routes.MapRoute(
                name: RouteNames.CommentForm,
                template: "comment/{slug}",
                defaults: new { controller = "home", action = "AddComment" });

            routes.MapRoute(
                name: RouteNames.NotFound,
                template: "notfound",
                defaults: new { controller = "home", action = "notfound" });

            // default 
            routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
        }

    }
}
