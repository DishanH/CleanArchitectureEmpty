using CleanArchitectureEmpty.Application.Common.Interfaces;
using CleanArchitectureEmpty.Infrastructure.Files;
using CleanArchitectureEmpty.Infrastructure.Identity;
using CleanArchitectureEmpty.Infrastructure.Persistence;
using CleanArchitectureEmpty.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;

namespace CleanArchitectureEmpty.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("CleanArchitectureEmptyDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<PasswordHasher<ApplicationUser>>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient(typeof(ICsvFileBuilder<>), typeof(CsvFileBuilder<>));


            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, UserRole.GlobalAdmin));
            // });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(action =>
            {
                action.RequireHttpsMetadata = false; //Only development
                action.SaveToken = true;
                action.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ////you might want to validate the audience and issuer depending on your use case
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(configuration["Jwt:Key"])),
                            ClockSkew = TimeSpan.Zero

                        };
            });


            return services;
        }
    }
}
