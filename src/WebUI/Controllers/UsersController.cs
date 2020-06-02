using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanArchitectureEmpty.Application.Common.Interfaces;

namespace CleanArchitectureEmpty.WebUI.Controllers
{

    [Authorize]
    public class UsersController : ApiController
    {
        private readonly IIdentityService _identityService;

        public UsersController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        [AllowAnonymous]
        [HttpPost(nameof(Authenticate))]
        public async Task<IActionResult> Authenticate()//[FromBody]LoginRequest login)
        {
            var result = await _identityService
            .Authenticate("administrator@localhost.com", "Administrator1!");

            if (result == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(result);
        }
    }
}