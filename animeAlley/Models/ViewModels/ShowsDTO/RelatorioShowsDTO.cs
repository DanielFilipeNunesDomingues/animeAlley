namespace animeAlley.Models.ViewModels.ShowsDTO
{

    public class RelatorioShowsDTO
    {
        public int TotalShows { get; set; }
        public int AnoInicio { get; set; }
        public int AnoFim { get; set; }
        public List<ShowsPorAnoDTO> ShowsPorAno { get; set; } = new();
        public List<ShowsPorGeneroDTO> ShowsPorGenero { get; set; } = new();
        public List<ShowsPorStatusDTO> ShowsPorStatus { get; set; } = new();
    }
}
