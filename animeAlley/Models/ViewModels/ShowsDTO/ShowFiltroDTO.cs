namespace animeAlley.Models.ViewModels.ShowsDTO
{
    public class ShowFiltroDTO
    {
        public string? Search { get; set; }
        public List<int>? GeneroIds { get; set; }
        public List<int>? StudioIds { get; set; }
        public List<int>? AutorIds { get; set; }
        public string? Status { get; set; }
        public decimal? NotaMinima { get; set; }
        public decimal? NotaMaxima { get; set; }
        public int? AnoInicio { get; set; }
        public int? AnoFim { get; set; }
        public string? Fonte { get; set; }
        public string OrdenarPor { get; set; } = "Nome"; // "Nome", "Nota", "Ano", "DataCriacao"
        public string Direcao { get; set; } = "ASC"; // "ASC", "DESC"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
