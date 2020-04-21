using System.Linq;
using CompanyEmployees.Data.Context;
using CompanyEmployees.Data.Repository;
// using CompanyEmployees.Formatters.Output;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
				opt.ReportApiVersions = true; opt.AssumeDefaultVersionWhenUnspecified = true; 
				opt.DefaultApiVersion = new ApiVersion(1, 0);
			});
		}
	}
}