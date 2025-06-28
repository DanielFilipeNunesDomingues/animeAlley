namespace animeAlley.Models.ViewModels
{
    // ================== DTOs PARA LOGIN ==================
    /// <summary>
    /// dados da pessoa para gerar uma autenticação
    /// </summary>
    public class LoginDTO
    {

        /// <summary>
        /// 'username' da pessoa que se quer autenticar
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// Password da pessoa que se quer autenticar
        /// </summary>
        public string Password { get; set; } = "";
    }
}
