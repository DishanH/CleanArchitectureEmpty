using CleanArchitectureEmpty.Application;
using CleanArchitectureEmpty.Application.Common.Interfaces;
using CleanArchitectureEmpty.Infrastructure;
using CleanArchitectureEmpty.Infrastructure.Persistence;
using CleanArchitectureEmpty.WebUI.Filters;
using CleanArchitectureEmpty.WebUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.IO;
using System.Reflection;
using System;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using MicroElements.Swashbuckle.FluentValidation;
using FluentValidation;
using Marvin.Cache.Headers;
using CleanArchitectureEmpty.WebUI.Configurations;

namespace CleanArchitectureEmpty.WebUI
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
            services.AddApplication();//Application layer dependencies
            services.AddInfrastructure(Configuration);//Infrastructure layer dependencies

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            services.AddControllers(options =>
            {
                //406 for unsupported media types
                options.ReturnHttpNotAcceptable = true;

                options.Filters.Add(new ApiExceptionFilter());
            })
            .AddXmlDataContractSerializerFormatters();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddHttpCacheHeaders(expirationModelOptions =>
            {
                expirationModelOptions.MaxAge = Convert.ToInt32(Configuration["Cache:MaxAge"]);
                expirationModelOptions.CacheLocation = CacheLocation.Private;
            }, validationModelOptions =>
            {
                validationModelOptions.MustRevalidate = true;
            });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.ReportApiVersions = true;
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";//v1.0
            });

            //Swagger support for FluentValidation MicroElements.Swashbuckle.FluentValidation          
            services.AddSingleton<IValidatorFactory, HttpContextServiceProviderValidatorFactory>();

            services.AddSwaggerGen();
            services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider desc)
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
            app.UseHttpCacheHeaders();

            app.UseHealthChecks("/health");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                //Multiple api definitions
                foreach (var item in desc.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint($"/swagger/CleanArchitectureEmptyAPISpecification{item.GroupName}/swagger.json",
                     item.GroupName.ToUpperInvariant());
                }
                setupAction.EnableFilter();
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
                //for Swagger UI branding
                //setupAction.InjectStylesheet("/Assets/custom-ui.css");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
