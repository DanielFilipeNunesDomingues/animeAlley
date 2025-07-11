namespace animeAlley.Models.ViewModels.EstatisticasDTO
{
    public class EstatisticaResumoDTO
    {
        public int TotalShows { get; set; }
        public int TotalUtilizadores { get; set; }
        public int TotalPersonagens { get; set; }
        public double NotaMediaGeral { get; set; }
        public string GeneroMaisPopular { get; set; } = string.Empty;
        public string ShowMaisPopular { get; set; } = string.Empty;
    }
}
