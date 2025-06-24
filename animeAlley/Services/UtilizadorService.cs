using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace animeAlley.Services
{
    public class UtilizadorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UtilizadorService(ApplicationDbContext context,
                               IHttpContextAccessor httpContextAccessor,
                               UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<string> GetNomeUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                return utilizador?.Nome ?? "Utilizador";
            }
            return string.Empty;
        }

        public async Task<string> GetFotoUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (utilizador?.Foto != null)
                {
                    if (utilizador.Foto == "placeholder.png" || utilizador.Foto == "avatar.png")
                    {
                        return "/img/" + utilizador.Foto;
                    }
                    else
                    {
                        return "/images/userFotos/" + utilizador.Foto;
                    }
                }
            }
            return "/img/placeholder.png"; // Default fallback
        }

        public async Task<Utilizador> GetUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                return await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);
            }
            return null;
        }

        public async Task<bool> IsAdminAsync()
        {
            var utilizador = await GetUtilizadorAsync();
            return utilizador?.isAdmin ?? false;
        }
    }
}