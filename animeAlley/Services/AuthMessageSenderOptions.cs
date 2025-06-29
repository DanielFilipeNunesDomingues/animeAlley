namespace animeAlley.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}