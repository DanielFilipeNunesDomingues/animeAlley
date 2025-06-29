using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtilizadoresController : Controller
    {
        /// <summary>
        /// Referência à Base de Dados do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UtilizadoresController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizadores.ToListAsync());
        }

        // GET: Utilizadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .Include(u => u.Lista)
                    .ThenInclude(l => l.ListaShows)
                        .ThenInclude(ls => ls.Show)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
            {
                return NotFound();
            }

            // Organizar os shows por status para facilitar a exibição
            var showsPorStatus = new Dictionary<ListaStatus, List<ListaShows>>();

            if (utilizador.Lista != null && utilizador.Lista.ListaShows.Any())
            {
                showsPorStatus = utilizador.Lista.ListaShows
                    .GroupBy(ls => ls.ListaStatus)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            ViewBag.ShowsPorStatus = showsPorStatus;

            return View(utilizador);
        }

        // GET: Utilizadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }

            // Buscar dados do Identity para preencher o formulário
            var identityUser = await _userManager.FindByNameAsync(utilizador.UserName);

            var viewModel = new EditUtilizadorViewModel
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Foto = utilizador.Foto,
                Banner = utilizador.Banner,
                isAdmin = utilizador.isAdmin,
                UserName = utilizador.UserName,
                Email = identityUser?.Email ?? string.Empty,
                CurrentFoto = utilizador.Foto,
                CurrentBanner = utilizador.Banner
            };

            return View(viewModel);
        }

        // POST: Utilizadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUtilizadorViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var utilizador = await _context.Utilizadores.FindAsync(id);
                    if (utilizador == null)
                    {
                        return NotFound();
                    }

                    // Buscar utilizador do Identity
                    var identityUser = await _userManager.FindByNameAsync(utilizador.UserName);
                    if (identityUser == null)
                    {
                        ModelState.AddModelError("", "Utilizador não encontrado no sistema de autenticação.");
                        // IMPORTANTE: Preservar dados para reexibir o formulário
                        model.CurrentFoto = utilizador.Foto;
                        model.CurrentBanner = utilizador.Banner;
                        return View(model);
                    }

                    string novaFoto = utilizador.Foto;
                    string novoBanner = utilizador.Banner;

                    // Processar upload de foto
                    if (model.RemoveFoto)
                    {
                        if (!string.IsNullOrEmpty(utilizador.Foto) && utilizador.Foto != "placeholder.png")
                        {
                            await RemoverImagemFisica(utilizador.Foto, "images/userFotos");
                        }
                        novaFoto = "placeholder.png";
                    }
                    else if (model.FotoFile != null)
                    {
                        if (!IsValidImageFile(model.FotoFile, 3 * 1024 * 1024))
                        {
                            ModelState.AddModelError("FotoFile", "Arquivo inválido. Use JPEG ou PNG com no máximo 3MB.");
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }

                        if (!string.IsNullOrEmpty(utilizador.Foto) && utilizador.Foto != "placeholder.png")
                        {
                            await RemoverImagemFisica(utilizador.Foto, "images/userFotos");
                        }

                        novaFoto = await SalvarImagemAsync(model.FotoFile, "images/userFotos");
                    }

                    // Processar upload de banner
                    if (model.RemoveBanner)
                    {
                        if (!string.IsNullOrEmpty(utilizador.Banner))
                        {
                            await RemoverImagemFisica(utilizador.Banner, "images/userBanners");
                        }
                        novoBanner = null;
                    }
                    else if (model.BannerFile != null)
                    {
                        if (!IsValidImageFile(model.BannerFile, 6 * 1024 * 1024))
                        {
                            ModelState.AddModelError("BannerFile", "Arquivo inválido. Use JPEG ou PNG com no máximo 6MB.");
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }

                        if (!string.IsNullOrEmpty(utilizador.Banner))
                        {
                            await RemoverImagemFisica(utilizador.Banner, "images/userBanners");
                        }

                        novoBanner = await SalvarImagemAsync(model.BannerFile, "images/userBanners");
                    }

                    // Atualizar dados na tabela Utilizadores
                    utilizador.Nome = model.Nome;
                    utilizador.Foto = novaFoto;
                    utilizador.Banner = novoBanner;
                    utilizador.isAdmin = model.isAdmin;

                    // Se o username mudou, atualizar em ambas as tabelas
                    if (utilizador.UserName != model.UserName)
                    {
                        // Verificar se o novo username já existe
                        var existingUser = await _userManager.FindByNameAsync(model.UserName);
                        if (existingUser != null && existingUser.Id != identityUser.Id)
                        {
                            ModelState.AddModelError("UserName", "Este nome de utilizador já está em uso.");
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }

                        // Atualizar no Identity
                        identityUser.UserName = model.UserName;
                        identityUser.NormalizedUserName = model.UserName.ToUpper();

                        var updateResult = await _userManager.UpdateAsync(identityUser);
                        if (!updateResult.Succeeded)
                        {
                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }

                        // Atualizar na tabela Utilizadores
                        utilizador.UserName = model.UserName;
                    }

                    // Se o email mudou, atualizar no Identity
                    if (identityUser.Email != model.Email)
                    {
                        // Verificar se o novo email já existe
                        var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);
                        if (existingEmailUser != null && existingEmailUser.Id != identityUser.Id)
                        {
                            ModelState.AddModelError("Email", "Este email já está em uso.");
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }

                        identityUser.Email = model.Email;
                        identityUser.NormalizedEmail = model.Email.ToUpper();

                        var updateEmailResult = await _userManager.UpdateAsync(identityUser);
                        if (!updateEmailResult.Succeeded)
                        {
                            foreach (var error in updateEmailResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            // IMPORTANTE: Preservar dados para reexibir o formulário
                            model.CurrentFoto = utilizador.Foto;
                            model.CurrentBanner = utilizador.Banner;
                            return View(model);
                        }
                    }

                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar utilizador: " + ex.Message);
                }
            }

            // Se chegamos aqui, algo correu mal - preservar dados para reexibir o formulário
            var utilizadorForModel = await _context.Utilizadores.FindAsync(id);
            if (utilizadorForModel != null)
            {
                model.CurrentFoto = utilizadorForModel.Foto;
                model.CurrentBanner = utilizadorForModel.Banner;
            }

            return View();
        }

        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .Include(u => u.Lista)
                    .ThenInclude(l => l.ListaShows)
                        .ThenInclude(ls => ls.Show)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
            {
                return NotFound();
            }

            // Buscar dados do Identity para exibição
            var identityUser = await _userManager.FindByNameAsync(utilizador.UserName);
            ViewBag.IdentityUserEmail = identityUser?.Email ?? "N/A";

            // Organizar os shows por status para facilitar a exibição
            var showsPorStatus = new Dictionary<ListaStatus, List<ListaShows>>();

            if (utilizador.Lista != null && utilizador.Lista.ListaShows.Any())
            {
                showsPorStatus = utilizador.Lista.ListaShows
                    .GroupBy(ls => ls.ListaStatus)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            ViewBag.ShowsPorStatus = showsPorStatus;

            return View(utilizador);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var utilizador = await _context.Utilizadores.FindAsync(id);
                if (utilizador == null)
                {
                    return NotFound();
                }

                // Buscar utilizador do Identity
                var identityUser = await _userManager.FindByNameAsync(utilizador.UserName);

                // Remover imagens físicas se existirem
                if (!string.IsNullOrEmpty(utilizador.Foto) && utilizador.Foto != "placeholder.png")
                {
                    await RemoverImagemFisica(utilizador.Foto, "images/userFotos");
                }

                if (!string.IsNullOrEmpty(utilizador.Banner))
                {
                    await RemoverImagemFisica(utilizador.Banner, "images/userBanners");
                }

                // Remover da tabela Utilizadores
                _context.Utilizadores.Remove(utilizador);
                await _context.SaveChangesAsync();

                // Remover do Identity (se existir)
                if (identityUser != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(identityUser);
                    if (!deleteResult.Succeeded)
                    {
                        // Log dos erros (implementar logging conforme necessário)
                        // Mas continuar, pois o utilizador já foi removido da tabela principal
                        TempData["WarningMessage"] = "Utilizador removido, mas houve problemas ao remover do sistema de autenticação.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Utilizador removido com sucesso!";
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Utilizador removido, mas não foi encontrado no sistema de autenticação.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao remover utilizador: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }

        private bool IsValidImageFile(IFormFile file, long maxSize)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > maxSize)
                return false;

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        private async Task<string> SalvarImagemAsync(IFormFile file, string pasta)
        {
            Guid g = Guid.NewGuid();
            string nomeImagem = g.ToString();
            string extensaoImagem = Path.GetExtension(file.FileName).ToLowerInvariant();
            nomeImagem += extensaoImagem;

            string localizacaoImagem = _webHostEnvironment.WebRootPath;
            localizacaoImagem = Path.Combine(localizacaoImagem, pasta);

            if (!Directory.Exists(localizacaoImagem))
            {
                Directory.CreateDirectory(localizacaoImagem);
            }

            string caminhoCompleto = Path.Combine(localizacaoImagem, nomeImagem);
            using (var streamImage = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(streamImage);
            }

            return nomeImagem;
        }

        private async Task RemoverImagemFisica(string nomeImagem, string pasta)
        {
            if (string.IsNullOrEmpty(nomeImagem))
                return;

            try
            {
                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                string caminhoCompleto = Path.Combine(localizacaoImagem, pasta, nomeImagem);

                if (System.IO.File.Exists(caminhoCompleto))
                {
                    System.IO.File.Delete(caminhoCompleto);
                }
            }
            catch (Exception)
            {
                // Silenciar erros de exclusão de arquivo
            }
        }

        /// <summary>
        /// Método auxiliar para obter o nome de exibição do status
        /// </summary>
        private string GetStatusDisplayName(ListaStatus status)
        {
            return status switch
            {
                ListaStatus.Assistir => "Assistindo",
                ListaStatus.Terminei => "Terminei",
                ListaStatus.Pausa => "Em Pausa",
                ListaStatus.Desisti => "Desisti",
                ListaStatus.Pensar_Assistir => "Planejo Assistir",
                _ => status.ToString()
            };
        }
    }

    // ViewModel para edição de utilizador
    public class EditUtilizadorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Foto atual")]
        public string? Foto { get; set; }

        [Display(Name = "Banner atual")]
        public string? Banner { get; set; }

        [Required]
        [Display(Name = "É Administrador")]
        public bool isAdmin { get; set; } = false;

        [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome de utilizador deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome de Utilizador")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Arquivo de Foto")]
        public IFormFile? FotoFile { get; set; }

        [Display(Name = "Arquivo de Banner")]
        public IFormFile? BannerFile { get; set; }

        [Display(Name = "Remover Foto")]
        public bool RemoveFoto { get; set; }

        [Display(Name = "Remover Banner")]
        public bool RemoveBanner { get; set; }

        // Propriedades auxiliares para exibição
        public string? CurrentFoto { get; set; }
        public string? CurrentBanner { get; set; }
    }
}