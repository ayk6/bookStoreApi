using System.Text;
using AspNetCoreRateLimit;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;

namespace WebApi.Extensions
{
	public static class ServicesExtension
	{
		public static void ConfigureSqlContext(this IServiceCollection services,
			IConfiguration configuration){
				services.AddDbContext<RepositoryContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
				}

		public static void ConfigureRepositoryManager(this IServiceCollection services)
		{
			services.AddScoped<IRepositoryManager, RepositoryManager>();
		}

		public static void ConfigureServiceManager(this IServiceCollection services)
		{
			services.AddScoped<IServiceManager, ServiceManager>();
		}

		public static void ConfigureLoggerService(this IServiceCollection services) =>
			services.AddSingleton<ILoggerService, LoggerManager>();

		public static void ConfigureActionFilters(this IServiceCollection services)
		{
			services.AddScoped<ValidationFilterAttribute>();
			services.AddSingleton<LogFilterAttribute>();
		}

		public static void ConfigureCors(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", builder =>
				builder.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod()
				.WithExposedHeaders("X-Pagination"));
			});
		}

		public static void ConfigureDataShaper(this IServiceCollection services)
		{
			services.AddScoped<IDataShaper<BookDTO>, DataShaper<BookDTO>>();

		}

		public static void ConfigureVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified=true;
				options.DefaultApiVersion = new ApiVersion(1,0);
				options.ApiVersionReader = new HeaderApiVersionReader("api-version");
				options.Conventions.Controller<BooksController>().HasApiVersion(new ApiVersion(1,0));
			});
		}

		public static void ConfigureResponseCaching(this IServiceCollection services)
		{
			services.AddResponseCaching();
		}

		public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
		{
			services.AddHttpCacheHeaders(expOptions =>
			{
				expOptions.MaxAge = 100;
				//expOptions.CacheLocation = CacheLocation.Private;
			},
			validationOptions => {
				validationOptions.MustRevalidate = false;
			});
		}

		public static void ConfigureRateLimitingOptions(this IServiceCollection services)
		{
			var rateLimitRules = new List<RateLimitRule>(){
				new RateLimitRule()
				{
					Endpoint = "*",
					Limit = 10,
					Period = "1m"
				},
			};
			services.Configure<IpRateLimitOptions>(option =>
			{
				option.GeneralRules = rateLimitRules;
			});
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
			services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
		}

		public static void ConfigureIdentity(this IServiceCollection services)
		{
			var builder = services.AddIdentity<User, IdentityRole>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;

				options.User.RequireUniqueEmail = true;
			})
				.AddEntityFrameworkStores<RepositoryContext>()
				.AddDefaultTokenProviders();
		}

		public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("JwtSettings");
			var secretKey = jwtSettings["secretKey"];

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["validIssuer"],
					ValidAudience = jwtSettings["validAudience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
				});
		}

		public static void ConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new OpenApiInfo { Title = "bsStoreApi", Version = "v1" });
				s.SwaggerDoc("v2", new OpenApiInfo { Title = "bsStoreApi", Version = "v2",
					Description = "bsStoreApi", TermsOfService = new Uri("https://github.com/"),
					Contact = new OpenApiContact
					{
						Name = "name",
						Email = "email",
						Url = new Uri("https://github.com/")
					}
					});

				s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					In = ParameterLocation.Header,
					Description = "Place to add jwt with bearer",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				s.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Name = "Bearer"
						},
						new List<string>()
					} 
				});
			});
		}

		public static void RegisterRepositories(this IServiceCollection services)
		{
			services.AddScoped<IBookRepository, BookRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();
		}

		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IBookService, BookManager>();
			services.AddScoped<ICategoryService, CategoryManager>();
			services.AddScoped<IAuthService, AuthManager>();
		}
	}
}
