using BLL.DTOs;
using BLL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace FlapKap_TechnicalChallenge.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("Fixed")]

    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly UserManager<User> _userManager;
        public ProductController( IProductService productService, UserManager<User> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }
        [HttpGet("GetAllProduct")]
        public IActionResult GetAllProduct()
        {
          var products=  _productService.GetAllProduct();
          
          if (products.IsNullOrEmpty())
                return NoContent();

          return Ok(products);

        }
        [HttpGet("GetProduct/{id}")]
        public async Task< IActionResult> GetProduct( string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productService.GetProductAsync(id);
            if (product.Message== "Product not found")
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }
        [Authorize(Roles ="Seller")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userName =await GetUserNameFromCookies();
          var Result = await _productService.AddProductAsync(productDto, userName);
          return Ok(Result);
        }
        [Authorize(Roles = "Seller")]
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userName = await GetUserNameFromCookies();
            var result =await _productService.UpdateProductAsync(productDto, userName);

            if (result.Message== "The Product not found")
            {
                return NotFound("The Product not found");
            }
            else if (result.Message== "You aren't allowed for updating this product")
            {
                return Forbid("You aren't allowed for updating this product");
            }
            return Ok(result);
        }

        [Authorize(Roles ="Seller")]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string username = await GetUserNameFromCookies();
            string result = await _productService.DeleteProductAsync(id, username);
            if (result == "The Product not found")
            {
                return NotFound("The Product not found");
            }
            else if (result == "You are not allowed to delete this product")
            {
                return StatusCode(403, "You are not allowed to delete this product");
            }
            return Ok(new {Message= result });
        }


        private async Task<string> GetUserNameFromCookies ()
        {
            string refToken = WebUtility.UrlDecode(Request.Cookies["RefreshToken"]);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refToken));

            return user.UserName;
        }

    }
}
