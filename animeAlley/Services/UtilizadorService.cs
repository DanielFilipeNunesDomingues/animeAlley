using animeAlley.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class UtilizadorService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UtilizadorService(ApplicationDbContext context,
                                 UserManager<IdentityUser> userManager,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> GetNomeUtilizadorAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return null;

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);

            return utilizador?.Nome;
        }
    }

}
