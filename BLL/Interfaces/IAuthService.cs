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
        Task<TokenResponseDTO> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
        Task<TokenResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
        Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);
    }
}
