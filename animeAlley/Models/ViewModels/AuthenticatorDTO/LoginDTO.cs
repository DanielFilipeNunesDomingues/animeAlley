using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.AuthenticatorDTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }
}
