using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;

namespace animeAlley.Models.ViewModels.EstatisticasDTO
{
    public class EstatisticaDTO
    {
        public int TotalShows { get; set; }
        public int TotalUtilizadores { get; set; }
        public int TotalGeneros { get; set; }
        public int TotalStudios { get; set; }
        public int TotalAutores { get; set; }
        public int TotalPersonagens { get; set; }
        public decimal NotaMediaGeral { get; set; }
        public List<GeneroPopularDTO> GenerosPopulares { get; set; } = new();
        public List<ShowPopularDTO> ShowsPopulares { get; set; } = new();
        public List<StudioPopularDTO> StudiosPopulares { get; set; } = new();
    }
}
