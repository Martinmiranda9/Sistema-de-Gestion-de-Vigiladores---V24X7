using SGV.DTOs.Auth;

namespace SGV.Business.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
}
