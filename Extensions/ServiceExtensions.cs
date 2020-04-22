using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CompanyEmployees.Data.Context;
using CompanyEmployees.Data.Repository;
using CompanyEmployees.Models;
// using CompanyEmployees.Formatters.Output;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CompanyEmployees.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureCors(this IServiceCollection services) =>
			services.AddCors(options =>
		{
			options.AddPolicy("CorsPolicy", builder =>
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());
		});

		public static void ConfigureLoggerService(this IServiceCollection services) =>
			services.AddScoped<ILoggerManager, LoggerManager>();

		public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
			services.AddDbContext<RepositoryContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

		public static void ConfigureRepositoryManager(this IServiceCollection services) =>
			services.AddScoped<IRepositoryManager, RepositoryManager>();

		// public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
		// 	builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

		public static void AddCustomMediaTypes(this IServiceCollection services)
		{
			services.Configure<MvcOptions>(config =>
			{
				var newtonsoftJsonOutputFormatter = config.OutputFormatters
					.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

				if (newtonsoftJsonOutputFormatter != null)
				{
					newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+json");
					newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+json");
				}
				var xmlOutputFormatter = config.OutputFormatters
					.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

				if (xmlOutputFormatter != null)
				{
					xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+xml");
					xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+xml");
				}
			});
		}

		public static void ConfigureVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(opt =>
			{
				opt.ReportApiVersions = true;
				opt.AssumeDefaultVersionWhenUnspecified = true;
				opt.DefaultApiVersion = new ApiVersion(1, 0);
				opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
			});
		}

		public static void ConfigureIdentity(this IServiceCollection services)
		{
			var builder = services.AddIdentityCore<User>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 10;
				options.User.RequireUniqueEmail = true;
			});

			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
			builder.AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
		}

		public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("JwtSettings");
			var secretKey = Environment.GetEnvironmentVariable("SECRET");

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
					ValidAudience = jwtSettings.GetSection("validAudience").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
				};
			});
		}

		public static void ConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Code Maze API",
					Version = "v1",
					Description = "CompanyEmployees API by Felipe Kosouski, Based on CodeMaze API Book",
					TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "John Doe",
						Email = "John.Doe@gmail.com",
						Url = new Uri("https://twitter.com/johndoe"),
					},
					License = new OpenApiLicense
					{
						Name = "CompanyEmployees API LICX",
						Url = new Uri("https://example.com/license"),
					}
				});
				s.SwaggerDoc("v2", new OpenApiInfo { Title = "Code Maze API", Version = "v2" });

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				s.IncludeXmlComments(xmlPath);
				
				s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Place to add JWT with Bearer",
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
						Name = "Bearer",
					},
					new List<string>()
				}
				});
			});
		}
	}
}