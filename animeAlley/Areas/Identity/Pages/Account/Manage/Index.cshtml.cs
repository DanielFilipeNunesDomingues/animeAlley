using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;

namespace animeAlley.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
            [Display(Name = "Nome")]
            public string Nome { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Foto atual")]
            public string? Foto { get; set; }

            [Display(Name = "Banner atual")]
            public string? Banner { get; set; }

            [Display(Name = "Arquivo de Avatar")]
            public IFormFile? AvatarFile { get; set; }

            [Display(Name = "Arquivo de Banner")]
            public IFormFile? BannerFile { get; set; }

            [Display(Name = "Remover Avatar")]
            public bool RemoveAvatar { get; set; }

            [Display(Name = "Remover Banner")]
            public bool RemoveBanner { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            // Buscar dados do utilizador na base de dados
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == userName);

            Input = new InputModel
            {
                Nome = utilizador?.Nome ?? string.Empty,
                PhoneNumber = phoneNumber,
                Foto = utilizador?.Foto,
                Banner = utilizador?.Banner
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Buscar utilizador na base de dados
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (utilizador == null)
            {
                StatusMessage = "Erro: Utilizador não encontrado na base de dados.";
                return RedirectToPage();
            }

            // Atualizar número de telefone
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Erro inesperado ao definir o número de telefone.";
                    return RedirectToPage();
                }
            }

            string novaFoto = utilizador.Foto;
            string novoBanner = utilizador.Banner;

            try
            {
                // Processar upload de avatar
                if (Input.RemoveAvatar)
                {
                    // Remover foto atual (exceto placeholder)
                    if (!string.IsNullOrEmpty(utilizador.Foto) && utilizador.Foto != "placeholder.png")
                    {
                        await RemoverImagemFisica(utilizador.Foto, "images/userFotos");
                    }
                    novaFoto = "placeholder.png";
                }
                else if (Input.AvatarFile != null)
                {
                    // Validar arquivo de avatar
                    if (!IsValidImageFile(Input.AvatarFile, 3 * 1024 * 1024)) // 3MB
                    {
                        ModelState.AddModelError("Input.AvatarFile", "Arquivo inválido. Use JPEG ou PNG com no máximo 3MB.");
                        await LoadAsync(user);
                        return Page();
                    }

                    // Remover foto anterior (exceto placeholder)
                    if (!string.IsNullOrEmpty(utilizador.Foto) && utilizador.Foto != "placeholder.png")
                    {
                        await RemoverImagemFisica(utilizador.Foto, "images/userFotos");
                    }

                    // Salvar nova foto
                    novaFoto = await SalvarImagemAsync(Input.AvatarFile, "images/userFotos");
                }

                // Processar upload de banner
                if (Input.RemoveBanner)
                {
                    // Remover banner atual
                    if (!string.IsNullOrEmpty(utilizador.Banner))
                    {
                        await RemoverImagemFisica(utilizador.Banner, "images/userBanners");
                    }
                    novoBanner = null;
                }
                else if (Input.BannerFile != null)
                {
                    // Validar arquivo de banner
                    if (!IsValidImageFile(Input.BannerFile, 6 * 1024 * 1024)) // 6MB
                    {
                        ModelState.AddModelError("Input.BannerFile", "Arquivo inválido. Use JPEG ou PNG com no máximo 6MB.");
                        await LoadAsync(user);
                        return Page();
                    }

                    // Remover banner anterior
                    if (!string.IsNullOrEmpty(utilizador.Banner))
                    {
                        await RemoverImagemFisica(utilizador.Banner, "images/userBanners");
                    }

                    // Salvar novo banner
                    novoBanner = await SalvarImagemAsync(Input.BannerFile, "images/userBanners");
                }

                // Atualizar dados na base de dados
                utilizador.Nome = Input.Nome;
                utilizador.Foto = novaFoto;
                utilizador.Banner = novoBanner;

                _context.Update(utilizador);
                await _context.SaveChangesAsync();

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "O seu perfil foi atualizado com sucesso.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log do erro (implementar logging conforme necessário)
                StatusMessage = "Erro: Não foi possível atualizar o perfil. Tente novamente.";

                // Reverter alterações se necessário
                // (as imagens já foram salvas, mas pode implementar lógica de rollback)

                return RedirectToPage();
            }
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
            // Gerar nome único para o arquivo (similar ao código de registo)
            Guid g = Guid.NewGuid();
            string nomeImagem = g.ToString();
            string extensaoImagem = Path.GetExtension(file.FileName).ToLowerInvariant();
            nomeImagem += extensaoImagem;

            // Definir localização da imagem
            string localizacaoImagem = _webHostEnvironment.WebRootPath;
            localizacaoImagem = Path.Combine(localizacaoImagem, pasta);

            // Criar diretório se não existir
            if (!Directory.Exists(localizacaoImagem))
            {
                Directory.CreateDirectory(localizacaoImagem);
            }

            // Salvar arquivo fisicamente
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
    }
}