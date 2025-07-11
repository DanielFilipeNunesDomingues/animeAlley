namespace animeAlley.Models.ViewModels.ShowsDTO
{
    public class ShowResumoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sinopse { get; set; }
        public string? Imagem { get; set; }
        public decimal? Nota { get; set; }
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> Generos { get; set; } = new();
        public string Studio { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
    }
}
