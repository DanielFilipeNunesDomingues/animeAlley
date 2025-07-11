using animeAlley.Models;
using animeAlley.Models.ViewModels.AuthenticatorDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<Utilizador?> GetUtilizadorByUserNameAsync(string userName);
        Task<bool> UpdateLastLoginAsync(string userName);
    }
}
