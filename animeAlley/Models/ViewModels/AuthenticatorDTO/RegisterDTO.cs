using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.AuthenticatorDTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "A confirmação de senha não confere")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Foto { get; set; }
        public string? Banner { get; set; }
    }
}
