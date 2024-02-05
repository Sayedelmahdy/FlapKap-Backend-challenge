using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<UserDetailDto> RegisterAsync(RegisterDto model);
        Task<AuthenticationDetailDto> GetTokenAsync(TokenRequestDto model);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<AuthenticationDetailDto> RefreshTokenAsync(string Token);
        Task<bool> RevokeTokenAsync(string Token);
    }
}
