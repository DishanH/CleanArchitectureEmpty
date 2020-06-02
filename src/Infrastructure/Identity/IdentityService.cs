using CleanArchitectureEmpty.Application.Common.Interfaces;
using CleanArchitectureEmpty.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitectureEmpty.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CleanArchitectureEmpty.Application.Common;

namespace CleanArchitectureEmpty.Infrastructure.Identity
{

    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public IDateTime _dateTime { get; }

        public IdentityService(
            ApplicationDbContext dbContext,
            IDateTime dateTime,
            PasswordHasher<ApplicationUser> passwordHasher,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _dateTime = dateTime;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return user?.UserName;
        }
        public async Task<bool> UserNameExistsAsync(string userName)
            => await _dbContext.Users.AnyAsync(u => u.UserName == userName);

        //public async Task<string> CreateUserAsync(string userName, string password,string role = null)
        public async Task<string> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _dbContext.Users.Add(user);
            //await _userManager.AddToRoleAsync(user, role); //For Roles
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<Dictionary<string, string>> Authenticate(string userName, string password)
        {
            var user = await _dbContext.Users.FirstAsync(u => u.UserName == userName);
            if (user == null)
                return null;
            var result = new Dictionary<string, string>();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var datetimNow = _dateTime.Now;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        //new Claim(ClaimTypes.Role,UserRole.GlobalAdmin),
                        new Claim(ClaimTypes.NameIdentifier,user.Id),
                        new Claim(ClaimTypes.Name,user.UserName)
                    }),
                NotBefore = datetimNow,
                IssuedAt = datetimNow,
                Expires = datetimNow.AddMinutes(Convert.ToInt16(_configuration["Jwt:ExpireMinutes"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            result.Add("accessToken", tokenHandler.WriteToken(token));
            result.Add("expiresIn", ((DateTime)tokenDescriptor.Expires - _dateTime.Now).TotalSeconds.ToString());
            //result.Add("refreshToken");//to-do

            return result;

        }

        // public async Task<Result> DeleteUserAsync(string userId)
        // {
        //     var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        //     if (user != null)
        //     {
        //         return await DeleteUserAsync(user);
        //     }

        //     return Result.Success();
        // }

        // public async Task<Result> DeleteUserAsync(ApplicationUser user)
        // {
        //     var result = await _userManager.DeleteAsync(user);

        //     return result.ToApplicationResult();
        // }
    }
}
