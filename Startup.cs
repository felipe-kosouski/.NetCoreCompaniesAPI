using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using AutoMapper;
using Contracts;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Contracts;
using CompanyEmployees.Dtos;
using CompanyEmployees.Data.DataShaping;
using CompanyEmployees.Utility;

namespace CompanyEmployees
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});
			services.AddScoped<ValidationFilterAttribute>();
			services.AddScoped<ValidateCompanyExistsAttribute>();
			services.AddScoped<ValidateEmployeeExistsAttribute>();
			services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
			services.AddScoped<ValidateMediaTypeAttribute>();
			services.AddScoped<EmployeeLinks>();
			services.ConfigureCors();
			services.ConfigureLoggerService();
			services.ConfigureSqlContext(Configuration);
			services.ConfigureRepositoryManager();
			services.AddControllers(config =>
			{
				config.RespectBrowserAcceptHeader = true;
				config.ReturnHttpNotAcceptable = true;
			})
				.AddNewtonsoftJson()
				.AddXmlDataContractSerializerFormatters();
			services.AddCustomMediaTypes();
			services.AddAutoMapper(typeof(Startup));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.ConfigureExceptionHandler(logger);
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCors("CorsPolicy");
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All
			});


			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
