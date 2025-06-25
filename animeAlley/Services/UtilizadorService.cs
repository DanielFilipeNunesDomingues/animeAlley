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
                if (!string.IsNullOrEmpty(userName))
                {
                    var utilizador = await _context.Utilizadores
                        .FirstOrDefaultAsync(u => u.UserName == userName);
                    return utilizador?.Nome ?? "Utilizador";
                }
            }
            return string.Empty;
        }

        public async Task<string> GetFotoUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
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
            }
            return "/img/placeholder.png"; // Default fallback
        }

        public async Task<int> GetIDUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    var utilizador = await _context.Utilizadores
                        .FirstOrDefaultAsync(u => u.UserName == userName);

                    // Verificação de null adicionada aqui
                    return utilizador?.Id ?? 0;
                }
            }
            return 0;
        }

        public async Task<Utilizador?> GetUtilizadorAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    return await _context.Utilizadores
                        .FirstOrDefaultAsync(u => u.UserName == userName);
                }
            }
            return null;
        }

        public async Task<bool> IsAdminAsync()
        {
            var utilizador = await GetUtilizadorAsync();
            return utilizador?.isAdmin ?? false;
        }

        /// <summary>
        /// Método auxiliar para obter o utilizador atual com verificações completas
        /// </summary>
        private async Task<Utilizador?> GetUtilizadorInternoAsync()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated != true)
                    return null;

                var userName = user.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return null;

                return await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);
            }
            catch (Exception)
            {
                // Log da exceção se necessário
                return null;
            }
        }

        /// <summary>
        /// Verifica se o utilizador atual existe no sistema
        /// </summary>
        public async Task<bool> UtilizadorExisteAsync()
        {
            var utilizador = await GetUtilizadorAsync();
            return utilizador != null;
        }

        /// <summary>
        /// Cria um utilizador no sistema baseado no Identity User
        /// </summary>
        public async Task<Utilizador?> CriarUtilizadorSeNaoExistirAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var userName = user.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return null;

            var utilizadorExistente = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (utilizadorExistente != null)
                return utilizadorExistente;

            // Criar novo utilizador
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser == null)
                return null;

            var novoUtilizador = new Utilizador
            {
                UserName = userName,
                Nome = identityUser.Email ?? userName, // Usar email ou username como nome padrão
                Foto = "placeholder.png",
                Banner = "bannerplaceholder.png",
                isAdmin = false
            };

            try
            {
                _context.Utilizadores.Add(novoUtilizador);
                await _context.SaveChangesAsync();
                return novoUtilizador;
            }
            catch (Exception)
            {
                // Log da exceção se necessário
                return null;
            }
        }
    }
}