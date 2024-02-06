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
    public class VendingMachineController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMachineTransactionsService _machineTransactionsService;
        public VendingMachineController(UserManager<User> userManager, IMachineTransactionsService machineTransactionsService)
        {
            _userManager = userManager;
            _machineTransactionsService = machineTransactionsService;
        }
        [Authorize(Roles ="Buyer")]
        [HttpPut("deposit/{coin}")]
        public async Task<IActionResult> Deposit(int coin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string userName = await GetUserNameFromCookies();
            string res = await _machineTransactionsService.Deposit(coin, userName);
            if (res.Contains("Invalid coin denomination"))
                return BadRequest(new { Message = res });
            return Ok(new { Message = res });
        }
        [Authorize(Roles = "Buyer")]
        [HttpPost("buy")]
        public async Task<IActionResult> Buy(PurchasedProductDto purchasedProductDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string userName = await GetUserNameFromCookies();
            var result = await _machineTransactionsService.Buy(purchasedProductDto, userName);
            if (result.Message== "The product not found / invalid Purchase")
                return NotFound(result);
            else if (result.Message.Contains("Insufficient stock for product"))
                return BadRequest(result);
            else if (result.Message== "Insufficient funds")
                return BadRequest(result);
            else if (result.Message== "Failed to complete the purchase")
                return StatusCode(500, result);
            return Ok(result);
        }
        [Authorize(Roles = "Buyer")]
        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            string userName = await GetUserNameFromCookies();
            string result = await _machineTransactionsService.Reset(userName);
            if (result== "Your deposit is already zero. There's nothing to reset")
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles ="Seller")]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw()
        {
            string userName = await GetUserNameFromCookies();
            var result = await _machineTransactionsService.Withdraw(userName);
            if (result.Message== "No money to withdraw")
                return BadRequest(result);
            return Ok(result);
        }
        private async Task<string> GetUserNameFromCookies()
        {
            string refToken = WebUtility.UrlDecode(Request.Cookies["RefreshToken"]);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refToken));

            return user.UserName;
        }
    }
}
