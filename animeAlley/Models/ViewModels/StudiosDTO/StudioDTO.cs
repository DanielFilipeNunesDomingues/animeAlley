namespace animeAlley.Models.ViewModels.StudiosDTO
{
    public class StudioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Foto { get; set; }
        public string? Sobre { get; set; }
        public DateTime? Fundado { get; set; }
        public DateTime? Fechado { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalShows { get; set; }
    }
}
