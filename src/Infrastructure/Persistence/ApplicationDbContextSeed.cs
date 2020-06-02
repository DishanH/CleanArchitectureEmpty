using CleanArchitectureEmpty.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureEmpty.Application.Common.Interfaces;

namespace CleanArchitectureEmpty.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(IIdentityService identityService)
        {
            var defaultUser = new ApplicationUser { UserName = "administrator@localhost.com" };

            if (!await identityService.UserNameExistsAsync(defaultUser.UserName))
            {
                await identityService.CreateUserAsync(defaultUser.UserName, "Administrator1!");
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            await Task.CompletedTask;
        }
    }
}
