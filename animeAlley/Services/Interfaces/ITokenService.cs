using animeAlley.Models;
using System.Security.Claims;

namespace animeAlley.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, Utilizador utilizador);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
