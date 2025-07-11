namespace animeAlley.Models.ViewModels.AuthenticatorDTO
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string? Foto { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
