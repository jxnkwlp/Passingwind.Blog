using AspNetCoreRateLimit;
using AutoMapper;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.OpenApi.Models;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.EventBus;
using Passingwind.Blog.Guids;
using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Impl;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Captcha;
using Passingwind.Blog.Web.Json;
using Passingwind.Blog.Web.Mvc.Json;
using Passingwind.Blog.Web.Razor;
using Passingwind.Blog.Web.Services;
using Passingwind.Blog.Web.UI.Theme;
using Passingwind.Blog.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

namespace Passingwind.Blog.Web
{
	public static class ServiceCollectionExtensions
	{
		private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(IEnumerable<Type> types, Type openGenericType)
		{
			return from x in types
				   from z in x.GetInterfaces()
				   let y = x.BaseType
				   where
				   (y != null && y.IsGenericType &&
				   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
				   (z.IsGenericType &&
				   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
				   select x;
		}

		private static TSetttings LoadSettings<TSetttings>(IServiceProvider services) where TSetttings : ISettings, new()
		{
			return AsyncHelper.RunSync(() => services.GetRequiredService<ISettingService>().LoadAsync<TSetttings>());
		}

		public static IServiceCollection ConfigureEventBus(this IServiceCollection services)
		{
			services.AddSingleton<IEventBus, InMemoryEventBus>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			var eventHandlerTypes = GetAllTypesImplementingOpenGenericType(assemblies.SelectMany(t => t.ExportedTypes), typeof(IEventBusHandler<>));

			foreach (var item in eventHandlerTypes)
			{
				var eventDataTypes = item.GetInterfaces().First(t => t.IsGenericType).GetGenericArguments();

				services.AddScoped(typeof(IEventBusHandler<>).MakeGenericType(eventDataTypes), item);
				services.AddScoped(item);
			}


			var sp = services.BuildServiceProvider().CreateScope().ServiceProvider;

			var eventBus = sp.GetRequiredService<IEventBus>();

			foreach (var handlerType in eventHandlerTypes)
			{
				var eventDataType = handlerType.GetInterfaces().First(t => t.IsGenericType).GetGenericArguments()[0];

				eventBus.GetType().GetMethod("Subscribe").MakeGenericMethod(eventDataType, handlerType).Invoke(eventBus, null);
			}

			return services;
		}

		public static IServiceCollection ConfigureSwaggerGenService(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Blog API", Version = "v1" });

				// Define the BearerAuth scheme that's in use
				options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
				{
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
				});

			});
			return services;
		}

		public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

			services.AddTransient<IEmailSender, EmailSender>();

			services.Scan(s =>
				s.FromDependencyContext(DependencyContext.Default)
				.AddClasses(t => t.AssignableTo<ISingletonDependency>())
					.AsImplementedInterfaces().WithSingletonLifetime()
				.AddClasses(t => t.AssignableTo<ITransientDependency>())
					.AsImplementedInterfaces().WithTransientLifetime()
				.AddClasses(t => t.AssignableTo<IScopedDependency>())
					.AsImplementedInterfaces().WithScopedLifetime()
			);

			services.AddSingleton<IJsonSerializer, JsonSerializer>();

			services.AddScoped<ICaptchaService, CaptchaService>();
			services.AddSingleton<IMarkdownService, DefaultMarkdownService>();

			services.AddTransient<BasicSettings>(LoadSettings<BasicSettings>);
			services.AddTransient<CommentsSettings>(LoadSettings<CommentsSettings>);
			services.AddTransient<FeedSettings>(LoadSettings<FeedSettings>);
			services.AddTransient<AdvancedSettings>(LoadSettings<AdvancedSettings>);
			services.AddTransient<EmailSettings>(LoadSettings<EmailSettings>);

			services.AddSingleton<IGuidGenerator, SequentialGuidGenerator>();

			services.AddTransient<IRazorViewService, RazorViewService>();

			services.AddScoped<IWidgetDynamicContentService, WidgetDynamicContentService>();


			return services;
		}

		public static IServiceCollection ConfigureAutoMapperService(this IServiceCollection services)
		{
			// services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			services.AddAutoMapper(AssemblyLoadContext.Default.Assemblies);

			return services;
		}

		public static IServiceCollection ConfigureCoreService(this IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

			});


			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddScoped<HttpApiResponseExceptionFilter>();
			services.AddScoped<HttpApiPermissionActionFilter>();

			services.AddMvcCore()
					.AddJsonOptions(options =>
					{
						options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy/MM/dd HH:mm:ss"));
						options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter("yyyy/MM/dd HH:mm:ss"));
					})
					.AddMvcOptions(options =>
					{
					})
					.ConfigureApiBehaviorOptions(options =>
					{
					})
					//.AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix) 
					.AddRazorRuntimeCompilation()
					.AddApiExplorer()
					;



			services.AddRazorPages();

			services.AddHttpContextAccessor();

			services.AddMemoryCache();
			services.AddDistributedMemoryCache();

			services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.Configure<RazorViewEngineOptions>(options =>
			{
				options.ViewLocationExpanders.Add(new BlogViewLocationExpander());
				options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
			});

			//services.Configure<RequestLocalizationOptions>(options =>
			//{
			//	options.DefaultRequestCulture = new RequestCulture("en-us");
			//	options.AddSupportedCultures("zh-cn", "en-us");
			//	options.AddSupportedUICultures("zh-cn", "en-us");
			//	options.SetDefaultCulture("zh-cn");
			//	options.FallBackToParentCultures = true;
			//	options.FallBackToParentUICultures = true;
			//	options.AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());
			//});

#if DEBUG
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(b => b.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true));
			});
#endif

			//services.AddRouting(options =>
			//{
			//	options.LowercaseUrls = true;
			//});

			services.AddResponseCaching();
			services.AddResponseCompression();

			return services;
		}

		public static IServiceCollection ConfigureIdenittyService(this IServiceCollection services, IConfiguration configuration, BlogOptions blogOptions)
		{
			services.AddIdentity<User, Role>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;

				options.SignIn.RequireConfirmedAccount = blogOptions.Account.RequireConfirmedAccount;
			})
				   .AddUserManager<BlogUserManager>()
				   .AddRoleManager<BlogRoleManager>()
				   .AddSignInManager<BlogSignInManager>()
				   .AddDefaultTokenProviders()
				   .AddEntityFrameworkStores<BlogDbContext>();

			services.AddScoped<CustomCookieAuthenticationEvents>();
			services.ConfigureApplicationCookie(options =>
			{
				options.EventsType = typeof(CustomCookieAuthenticationEvents);
			});

			// default add by identity
			// services.AddAuthentication();

			var authenticationBuilder = services.AddAuthentication();

			services.Configure<AuthenticationOptions>(options =>
			{
				// options.DefaultScheme = "Cookies"; 
			});

			if (configuration.GetValue<bool>("Authentication:Microsoft:Enabled"))
			{
				// test ok
				authenticationBuilder.AddMicrosoftAccount(options =>
				{
					options.ClientId = configuration["Authentication:Microsoft:ClientId"];
					options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
					options.CallbackPath = new PathString("/signin-microsoft");
					options.AccessDeniedPath = new PathString("/account/accessdenied");
				});
			}
			if (configuration.GetValue<bool>("Authentication:GitHub:Enabled"))
			{
				// test ok
				authenticationBuilder.AddGitHub(options =>
				{
					options.ClientId = configuration["Authentication:GitHub:ClientId"];
					options.ClientSecret = configuration["Authentication:GitHub:ClientSecret"];
					options.CallbackPath = new PathString("/signin-github");
					options.AccessDeniedPath = new PathString("/account/accessdenied");
				});
			}
			if (configuration.GetValue<bool>("Authentication:Google:Enabled"))
			{
				// test ok
				authenticationBuilder.AddGoogle(options =>
				{
					options.ClientId = configuration["Authentication:Google:ClientId"];
					options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
					options.CallbackPath = new PathString("/signin-google");
					options.AccessDeniedPath = new PathString("/account/accessdenied");
				});
			}
			if (configuration.GetValue<bool>("Authentication:AzureAD:Enabled"))
			{
				// test ok
				authenticationBuilder.AddAzureAD(options =>
				{
					options.TenantId = configuration["Authentication:AzureAD:TenantId"];
					options.ClientId = configuration["Authentication:AzureAD:ClientId"];
					options.Instance = configuration["Authentication:AzureAD:Instance"];
					options.Domain = configuration["Authentication:AzureAD:Domain"];
					options.ClientSecret = configuration["Authentication:AzureAD:ClientSecret"];
					options.CallbackPath = new PathString("/signin-azuread");
					options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
					options.CookieSchemeName = "Identity.External"; // import
				});
				services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
				{
					options.Authority = options.Authority + "/v2.0/";

					// Per the code below, this application signs in users in any Work and School
					// accounts and any Microsoft Personal Accounts.
					// If you want to direct Azure AD to restrict the users that can sign-in, change 
					// the tenant value of the appsettings.json file in the following way:
					// - only Work and School accounts => 'organizations'
					// - only Microsoft Personal accounts => 'consumers'
					// - Work and School and Personal accounts => 'common'

					// If you want to restrict the users that can sign-in to only one tenant
					// set the tenant value in the appsettings.json file to the tenant ID of this
					// organization, and set ValidateIssuer below to true.

					// If you want to restrict the users that can sign-in to several organizations
					// Set the tenant value in the appsettings.json file to 'organizations', set
					// ValidateIssuer, above to 'true', and add the issuers you want to accept to the
					// options.TokenValidationParameters.ValidIssuers collection
					options.TokenValidationParameters.ValidateIssuer = false;

				});
			}


			if (configuration.GetValue<bool>("Authentication:OpenIdConnect:Enabled"))
			{
				authenticationBuilder.AddOpenIdConnect(options =>
				{
					options.CallbackPath = new PathString("/signin-oidc");
					options.AccessDeniedPath = new PathString("/account/accessdenied");
					options.ClientId = configuration["Authentication:OpenIdConnect:ClientId"];
					options.ClientSecret = configuration["Authentication:OpenIdConnect:ClientSecret"];
					options.Authority = configuration["Authentication:OpenIdConnect:Authority"];
					options.ResponseType = configuration["Authentication:OpenIdConnect:ResponseType"];
					options.RequireHttpsMetadata = false;
					options.Scope.Add("openid");
					options.Scope.Add("email");
				});
			}


			services.AddAuthorization(options =>
			{
			});

			return services;
		}

		public static IServiceCollection ConfigureMiniProfiler(this IServiceCollection services)
		{
			services.AddMiniProfiler()
					.AddEntityFramework();

			return services;
		}

		public static IServiceCollection ConfigureEasyCaching(this IServiceCollection services, RedisOptions redisOptions)
		{
			services.AddEasyCaching(options =>
			{
				//use memory cache that named default
				options.UseInMemory("default");

				if (redisOptions.Enabled)
				{
					options.UseRedis(config =>
					{
						config.DBConfig.Endpoints.Add(new ServerEndPoint(redisOptions.Server, redisOptions.Port));
						config.DBConfig.Database = redisOptions.Database;
						config.DBConfig.Password = redisOptions.Password;
					}, "redis");
				}
				else
				{

				}
			});

			return services;
		}

		public static IServiceCollection ConfigureRateLimit(this IServiceCollection services, IConfiguration configuration)
		{
			//load general configuration from appsettings.json
			services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

			//load ip rules from appsettings.json
			services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

			// inject counter and rules stores
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

			// configuration (resolvers, counter key builders)
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

			//// inject counter and rules distributed cache stores
			//services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
			//services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

			return services;
		}
	}
}
