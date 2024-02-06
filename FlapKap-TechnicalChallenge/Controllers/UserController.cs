using BLL.DTOs;
using BLL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FlapKap_TechnicalChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userManager = userManager;
           _userService=userService;
        }
        [HttpGet]        
        public async Task<IActionResult> GetUser ()
        {
            string userName = await GetUserNameFromCookies();
            var res = await _userService.GetUser (userName);
            return Ok (res);
        }
        

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser (EditUserDto model)
        {
            if (!ModelState.IsValid)
            {  
                return BadRequest(ModelState);
            }
            string userName = await GetUserNameFromCookies();
            var result = await _userService.UpdateUserAsync(userName, model);

            return Ok(result);

        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            string userName = await GetUserNameFromCookies();
            bool result = await _userService.DeleteUserAsync(userName);
            return Ok(new {Message= "User deleted successfully" });
        }

        private async Task<string> GetUserNameFromCookies()
        {
            string refToken = WebUtility.UrlDecode(Request.Cookies["RefreshToken"]);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refToken));

            return user.UserName;
        }
    }
}
