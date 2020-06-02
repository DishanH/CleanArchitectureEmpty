using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArchitectureEmpty.WebUI.Configurations
{
public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        }
        public void Configure(SwaggerGenOptions setupAction)
        {
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                setupAction.SwaggerDoc($"CleanArchitectureEmptyAPISpecification{description.GroupName}",
                    new OpenApiInfo
                    {
                        Title = "CleanArchitectureEmpty API",
                        Version = description.ApiVersion.ToString()
                    });
                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentFilePath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

                setupAction.IncludeXmlComments(xmlCommentFilePath, true);

                setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var apiVersionModel = apiDescription.ActionDescriptor
                    .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                    if (apiVersionModel == null)
                        return true;

                    if (apiVersionModel.DeclaredApiVersions.Any())
                    {
                        return apiVersionModel.DeclaredApiVersions.Any(v =>
                        {
                            var version = v.ToString();
                            return ($"CleanArchitectureEmptyAPISpecificationv{v.ToString()}" == documentName);
                        });
                    }
                    return apiVersionModel.ImplementedApiVersions.Any(v =>
                    {
                        return ($"CleanArchitectureEmptyAPISpecificationv{v.ToString()}" == documentName);
                    });
                });
                setupAction.ResolveConflictingActions(apiDescriptions =>
                {
                    return apiDescriptions.First();
                });

                setupAction.AddSecurityDefinition($"SecurityDefinition{description.GroupName}",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        Description = "Type into the textbox: Bearer {your JWT token}.",
                        Name = "Authorization",
                        In = ParameterLocation.Header

                    });

                setupAction.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id =$"SecurityDefinition{description.GroupName}"
                                    }
                                },
                                new List<string>()
                            }
                    }
                );
                setupAction.AddFluentValidationRules();
            }
        }
    
        
    }
}