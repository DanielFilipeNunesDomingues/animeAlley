// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace animeAlley.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// este objeto será usado para fazer a transposição de dados entre este
        /// ficheiro (de programação) e a sua respetiva visualização
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// se for instanciado, este atributo terá o link para onde a aplicação
        /// será redirecionada, após Registo
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Se estiver especificado a Autenticação por outros fornecedores
        /// de autenticação, este atributo terá essa lista de outros fornecedores
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// define os atributos que estarão presentes na interface da página
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// email do novo utilizador
            /// </summary>
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
            [EmailAddress(ErrorMessage = "Tem de escrever um {0} válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///  password associada ao utilizador
            /// </summary>
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
            [StringLength(20, ErrorMessage = "A {0} tem de ter, pelo menos, {2} e um máximo de {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            /// confirmação da password
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare(nameof(Password), ErrorMessage = "A password e a sua confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            /// Código Admin para tornar o utilizador administrador
            /// </summary>
            [Display(Name = "Código de Administrador (opcional)")]
            public string CodigoAdmin { get; set; }

            /// <summary>
            /// Foto do utilizador (opcional)
            /// </summary>
            [Display(Name = "Foto de Perfil (opcional)")]
            public IFormFile FotoUtilizador { get; set; }

            /// <summary>
            /// Incorporação dos dados de um Utilizador
            /// no formulário de Registo
            /// </summary>
            public Utilizador Utilizador { get; set; }
        }

        /// <summary>
        /// Este método 'responde' aos pedidos feitos em HTTP GET
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// Este método 'responde' aos pedidos do browser, quando feitos em HTTP POST
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // se o 'returnUrl' for nulo, é-lhe atribuído o valor da 'raiz' da aplicação
            returnUrl ??= Url.Content("~/");

            // O ModelState avalia o estado do objeto da classe interna 'InputModel'
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // atribuir ao objeto 'user' o email e o username
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // guardar os dados do 'user' na BD, juntando-lhe a password
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // se chegar aqui, consegui escrever os dados do novo utilizador na
                    // tabela AspNetUsers

                    /* ++++++++++++++++++++++++++++++++++++ */
                    // guardar os dados do Utilizador na BD
                    /* ++++++++++++++++++++++++++++++++++++ */

                    // var auxiliar
                    bool haErro = false;
                    string imagePath = "placeholder.png"; // Default placeholder

                    // atribuir o UserName do utilizador AspNetUser criado 
                    // ao objeto Utilizador
                    Input.Utilizador.UserName = Input.Email;

                    // Verificar se é administrador usando código
                    const string CODIGO_ADMIN = "ADMIN2025"; // Altere este código conforme necessário
                    if (!string.IsNullOrEmpty(Input.CodigoAdmin) && Input.CodigoAdmin == CODIGO_ADMIN)
                    {
                        Input.Utilizador.isAdmin = true;
                    }

                    // Processar a foto do utilizador se foi enviada
                    if (Input.FotoUtilizador != null)
                    {
                        if (Input.FotoUtilizador.ContentType == "image/jpeg" || Input.FotoUtilizador.ContentType == "image/png")
                        {
                            // Gerar nome criptografado para a imagem
                            Guid g = Guid.NewGuid();
                            imagePath = g.ToString();
                            string extensaoImagem = Path.GetExtension(Input.FotoUtilizador.FileName).ToLowerInvariant();
                            imagePath += extensaoImagem;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Formato de imagem inválido. Use apenas JPEG ou PNG.");
                            return Page();
                        }
                    }

                    // Atribuir o caminho da foto ao utilizador
                    Input.Utilizador.Foto = imagePath;

                    try
                    {
                        _context.Add(Input.Utilizador);
                        await _context.SaveChangesAsync();

                        // Adicionar role ao utilizador
                        if (Input.Utilizador.isAdmin)
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                            await _signInManager.RefreshSignInAsync(user);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }

                        // Se uma foto foi enviada, salvá-la fisicamente
                        if (Input.FotoUtilizador != null && !imagePath.Equals("placeholder.png"))
                        {
                            string localizacaoImagem = _webHostEnvironment.WebRootPath;
                            localizacaoImagem = Path.Combine(localizacaoImagem, "images/userFotos");

                            if (!Directory.Exists(localizacaoImagem))
                            {
                                Directory.CreateDirectory(localizacaoImagem);
                            }

                            string caminhoCompleto = Path.Combine(localizacaoImagem, imagePath);
                            using var streamImage = new FileStream(caminhoCompleto, FileMode.Create);
                            await Input.FotoUtilizador.CopyToAsync(streamImage);
                        }
                    }
                    catch (Exception)
                    {
                        /// NÃO ESQUECER!!!!
                        /// HÁ NECESSIDADE DE TRATAR ESTE ERRO!!!!
                        /// Se chegam aqui é pq não conseguiram guardar os dados
                        /// o que fazer?
                        ///    - apagar o user AspNetUser já criado
                        ///    - escrever um 'log' no disco rígido do Servidor
                        ///    - escrever o erro numa tabela da BD (o que pode 
                        ///         não ser possível, dependendo da exceção gerada pela BD)
                        ///    - enviar email para o Adminstrador da aplicação a relatar este facto 
                        ///    - notificar app que há erro
                        ///    - executar outras ações consideradas necessárias
                        ///          (eventualmente, eliminar a instrução 'throw'
                        haErro = true;

                        // Apagar o usuário do Identity se não conseguiu salvar na BD
                        await _userManager.DeleteAsync(user);
                        throw;
                    }
                    /* ++++++++++++++++++++++++++++++++++++ */

                    if (!haErro)
                    {
                        // Obter o ID do novo utilizador
                        var userId = await _userManager.GetUserIdAsync(user);
                        // obter o Código a ser enviado para o email do novo utilizador
                        // para validar o email, e codificá-lo em UTF8
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        // cria o link a ser enviado para o email, que há-de possibilitar a
                        // validação do email
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        // criar o email e enviá-lo
                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // Se tiver sido definido que o Registo deve ser seguido de validação do
                        // email, redireciona para a página de Confirmação de Registo de um novo Utilizador
                        // este parâmetro está escrito no ficheiro 'Program.cs'
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                }
                else
                {
                    // se há erros, mostra-os na página de Registo
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return Page();
        }

        /// <summary>
        /// Cria um objeto vazio do tipo IdentityUser
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}