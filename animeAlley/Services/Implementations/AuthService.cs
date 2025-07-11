using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.AuthenticatorDTO;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !user.EmailConfirmed)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return null;

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

            if (utilizador == null)
                return null;

            // Atualizar último login
            await UpdateLastLoginAsync(loginDto.UserName);

            var token = _tokenService.GenerateToken(user, utilizador);

            return new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Nome = utilizador.Nome,
                IsAdmin = utilizador.isAdmin,
                Foto = utilizador.Foto,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Verificar se o usuário já existe
            var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
                return null;

            var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingEmail != null)
                return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Criar registro na tabela Utilizador
                var utilizador = new Utilizador
                {
                    Nome = registerDto.Nome,
                    UserName = registerDto.UserName,
                    Foto = registerDto.Foto,
                    Banner = registerDto.Banner,
                    isAdmin = false, // Novos usuários não são admin por padrão
                };

                _context.Utilizadores.Add(utilizador);
                await _context.SaveChangesAsync();

                // Criar usuário no Identity
                var user = new ApplicationUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    EmailConfirmed = true, // Para simplificar, confirme automaticamente
                    UtilizadorId = utilizador.Id,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();

                var token = _tokenService.GenerateToken(user, utilizador);

                return new AuthResponseDto
                {
                    Token = token,
                    UserName = user.UserName,
                    Email = user.Email,
                    Nome = utilizador.Nome,
                    IsAdmin = utilizador.isAdmin,
                    Foto = utilizador.Foto,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Utilizador?> GetUtilizadorByUserNameAsync(string userName)
        {
            return await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> UpdateLastLoginAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user != null && utilizador != null)
            {
                user.LastLogin = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
