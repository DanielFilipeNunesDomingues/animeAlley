// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace animeAlley.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "A senha é obrigatória para confirmar a exclusão.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o utilizador com ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o utilizador com ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Senha incorreta.");
                    return Page();
                }
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var userEmail = await _userManager.GetEmailAsync(user);

            try
            {
                // 1. PRIMEIRO: Buscar o utilizador na sua base de dados
                var utilizador = await _context.Utilizadores
                    .Include(u => u.Lista) // Incluir dados relacionados se necessário
                    .FirstOrDefaultAsync(u => u.UserName == userEmail);

                if (utilizador != null)
                {
                    // 2. Apagar ficheiros de imagem se existirem
                    await ApagarFicheirosUtilizador(utilizador);

                    // 3. Apagar dados relacionados (se houver)
                    // Por exemplo, se tiver uma Lista associada
                    if (utilizador.Lista != null)
                    {
                        _context.Listas.Remove(utilizador.Lista);
                    }

                    // 4. Apagar o utilizador da sua base de dados
                    _context.Utilizadores.Remove(utilizador);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Dados do utilizador {Email} removidos da base de dados personalizada.", userEmail);
                }

                // 5. DEPOIS: Apagar da base de dados do Identity
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    // Se falhou a eliminação do Identity, tentar reverter as alterações
                    throw new InvalidOperationException($"Erro inesperado ao eliminar utilizador do Identity: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await _signInManager.SignOutAsync();

                _logger.LogInformation("Utilizador com ID '{UserId}' e email '{Email}' eliminou a sua conta completamente.", userId, userEmail);

                // Redirecionar para página de confirmação ou homepage
                TempData["SuccessMessage"] = "A sua conta foi eliminada com sucesso.";
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao eliminar conta do utilizador {Email}", userEmail);
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao eliminar a sua conta. Tente novamente mais tarde.");
                return Page();
            }
        }

        /// <summary>
        /// Apaga os ficheiros de imagem (foto e banner) do utilizador
        /// </summary>
        /// <param name="utilizador">O utilizador cujos ficheiros serão apagados</param>
        private Task ApagarFicheirosUtilizador(Utilizador utilizador)
        {
            try
            {
                string caminhoImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images/userFotos");

                // Apagar foto do utilizador se não for o placeholder
                if (!string.IsNullOrEmpty(utilizador.Foto) &&
                    !utilizador.Foto.Equals("placeholder.png", StringComparison.OrdinalIgnoreCase))
                {
                    string caminhoFoto = Path.Combine(caminhoImagens, utilizador.Foto);
                    if (System.IO.File.Exists(caminhoFoto))
                    {
                        System.IO.File.Delete(caminhoFoto);
                        _logger.LogInformation("Foto {FotoPath} eliminada.", utilizador.Foto);
                    }
                }

                // Apagar banner do utilizador se não for o placeholder
                if (!string.IsNullOrEmpty(utilizador.Banner) &&
                    !utilizador.Banner.Equals("bannerplaceholder.png", StringComparison.OrdinalIgnoreCase))
                {
                    string caminhoBanner = Path.Combine(caminhoImagens, utilizador.Banner);
                    if (System.IO.File.Exists(caminhoBanner))
                    {
                        System.IO.File.Delete(caminhoBanner);
                        _logger.LogInformation("Banner {BannerPath} eliminado.", utilizador.Banner);
                    }
                }
            }
            catch (Exception ex)
            {
                // Não falhar a eliminação da conta por causa dos ficheiros
                _logger.LogWarning(ex, "Erro ao eliminar ficheiros do utilizador {UserName}", utilizador.UserName);
            }

            return Task.CompletedTask;
        }
    }
}