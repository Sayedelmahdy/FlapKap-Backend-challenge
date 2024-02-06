using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
   public interface IUserService
    {
        Task<UserDetailDto>  GetUser(string username);
        Task<UserDetailDto> UpdateUserAsync(string Username, EditUserDto model);
        Task<bool> DeleteUserAsync(string Username);

    }
}
