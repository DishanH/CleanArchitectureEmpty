using CleanArchitectureEmpty.Application.Common.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CleanArchitectureEmpty.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<bool> UserNameExistsAsync(string userName);
        Task<Dictionary<string, string>> Authenticate(string email, string password);
        Task<string> CreateUserAsync(string username, string password);
    }
}
