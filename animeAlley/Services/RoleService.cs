using Microsoft.AspNetCore.Identity;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class RoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RoleService(UserManager<ApplicationUser> userManager,
                          RoleManager<IdentityRole> roleManager, 
                          ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        /// Adiciona um utilizador à role Admin
        /// </summary>
        public async Task<bool> AddUserToAdminRoleAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");

                if (result.Succeeded)
                {
                    // Atualizar também na tabela personalizada
                    var utilizador = await _context.Utilizadores
                        .FirstOrDefaultAsync(u => u.UserName == userEmail);

                    if (utilizador != null)
                    {
                        utilizador.isAdmin = true;
                        await _context.SaveChangesAsync();
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove um utilizador da role Admin
        /// </summary>
        public async Task<bool> RemoveUserFromAdminRoleAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

                if (result.Succeeded)
                {
                    // Atualizar também na tabela personalizada
                    var utilizador = await _context.Utilizadores
                        .FirstOrDefaultAsync(u => u.UserName == userEmail);

                    if (utilizador != null)
                    {
                        utilizador.isAdmin = false;
                        await _context.SaveChangesAsync();
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica se um utilizador é Admin
        /// </summary>
        public async Task<bool> IsUserAdminAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                return await _userManager.IsInRoleAsync(user, "Admin");
            }
            return false;
        }

        /// <summary>
        /// Sincroniza roles do Identity com a tabela personalizada
        /// </summary>
        public async Task SyncRolesWithCustomTableAsync()
        {
            var utilizadores = await _context.Utilizadores.ToListAsync();

            foreach (var utilizador in utilizadores)
            {
                var user = await _userManager.FindByEmailAsync(utilizador.UserName);
                if (user != null)
                {
                    var isInAdminRole = await _userManager.IsInRoleAsync(user, "Admin");

                    if (utilizador.isAdmin && !isInAdminRole)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else if (!utilizador.isAdmin && isInAdminRole)
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                    }
                }
            }
        }

        /// <summary>
        /// Cria uma role se ela não existir
        /// </summary>
        public async Task<bool> CreateRoleIfNotExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                return result.Succeeded;
            }
            return true;
        }

        /// <summary>
        /// Obtém todas as roles disponíveis
        /// </summary>
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        /// <summary>
        /// Obtém as roles de um utilizador
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            return new List<string>();
        }
    }
}