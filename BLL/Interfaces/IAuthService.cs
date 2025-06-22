using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<TokenResponseDTO> LoginAsync(LoginDTO dto);
        Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken);
        Task ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task ResetPasswordAsync(ResetPasswordDTO dto);
    }
}
