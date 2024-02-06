using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Text.Json;

namespace FlapKap_TechnicalChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("Fixed")]
    public class AuthenticationController : ControllerBase
    {
         private IAuthService _authService { get; }
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (result.Message != "User Created Successfully")
            {
                return BadRequest(JsonSerializer.Serialize( result.Message));
            }
            

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] TokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(JsonSerializer.Serialize( result.Message));

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }


        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["RefreshToken"];
            if (refreshToken==null)
            {
                return BadRequest(new { message = "You need to login first" });
            }

            var result = await _authService.RefreshTokenAsync(WebUtility.UrlDecode(refreshToken));

            if (!result.IsAuthenticated)
                return BadRequest(result);


            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token)) return BadRequest(new { message = "Token is required!" }); 
            var token2 = WebUtility.UrlDecode(token);
            var result = await _authService.RevokeTokenAsync(token2);


            if (!result) return BadRequest("Token is invalid!"); 

            Response.Cookies.Delete("refreshToken");
            return Ok(new {message = "Token Revoked and cookies Deleted" });
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)//push to cookie
        {
            var encodedToken = WebUtility.UrlEncode(refreshToken);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("RefreshToken", encodedToken, cookieOptions);


        }
    }
}
