namespace animeAlley.Models.ViewModels.AutoresDTO
{
    public class AutorDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sobre { get; set; }
        public string? Foto { get; set; }
        public string? Sexo { get; set; }
        public int? Idade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int TotalObras { get; set; }
    }
}
