using BLL.DTOs;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;

        }
        public async Task<UserDetailDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new UserDetailDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new UserDetailDto { Message = "Username is already registered!" };

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new UserDetailDto { Message = errors };
            }
            if (model.Role.Count() > 0)
            {
                foreach (var role in model.Role)
                {
                    if (model.Role.FirstOrDefault().ToLower() == "buyer")
                        await _userManager.AddToRoleAsync(user, "Buyer");
                    else if (model.Role.FirstOrDefault().ToLower() == "seller")
                        await _userManager.AddToRoleAsync(user, "Seller");



                }
            }


            else return new UserDetailDto { Message = "Invalid Role" };


            return new UserDetailDto
            {
                Message = $"User Created Successfully",
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
            };
        }


        // login
        public async Task<AuthenticationDetailDto> GetTokenAsync(TokenRequestDto model)
        {


            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthenticationDetailDto { Message = "Email or Password is incorrect!" };

            }
            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);
            var Auth = new AuthenticationDetailDto
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = rolesList.ToList()
            };
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                Auth.RefreshToken = activeRefreshToken.Token;
                Auth.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var NewRefreshToken = GenerateRefreshToken();
                Auth.RefreshToken = NewRefreshToken.Token;
                Auth.RefreshTokenExpiration = NewRefreshToken.ExpiresOn;
                user.RefreshTokens.Add(NewRefreshToken);
                await _userManager.UpdateAsync(user);
            }
            return Auth;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var Random = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(Random);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(Random),
                ExpiresOn = DateTime.Now.AddDays(7),//numofdays
                CreatedOn = DateTime.Now
            };
        }
        public async Task<AuthenticationDetailDto> RefreshTokenAsync(string Token)
        {
            var auth = new AuthenticationDetailDto();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));

            if (user == null)
            {
                auth.Message = "Invalid token / Need To Login First";
                return auth;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);

            if (!refreshToken.IsActive)
            {
                auth.Message = "Inactive token / Token Expaired You Need To login";
                return auth;
            }

            refreshToken.RevokedOn = DateTime.Now;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            auth.IsAuthenticated = true;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            auth.Email = user.Email;
            auth.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            auth.Roles = roles.ToList();
            auth.RefreshToken = newRefreshToken.Token;
            auth.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            return auth;
        }
        //Logout
        public async Task<bool> RevokeTokenAsync(string Token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);

            if (!refreshToken.IsActive) return false;
            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            return true;
        }
    }
}
