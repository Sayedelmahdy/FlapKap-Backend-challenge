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
    internal class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
           
        }

       

        public async Task<UserDetailDto> UpdateUserAsync(string Username, EditUserDto model)
        {
            var user = await _userManager.FindByNameAsync(Username);
            if (user is null)
                return new UserDetailDto { Message = "Username is Not Found" };



            if (model.Role.Count() > 0)
            {
                var role = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, role);
                await _userManager.AddToRolesAsync(user, model.Role);

            }
           
            if (model.Email != null) { 
               user.Email = model.Email;
            }
            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (model.Password != null)
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password);
            }
            if (model.Username != null)
            {
                user.UserName = model.Username;
            }
           
            await _userManager.UpdateAsync(user);



            return new UserDetailDto
            {
                Message = $"{user.UserName} is Updated",
                UserName = user.UserName,
                Email = user.Email,
                Role = await _userManager.GetRolesAsync(user),
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> DeleteUserAsync(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                return false;
            }
           await _userManager.DeleteAsync(user);
            return true;
        }
    }
}
