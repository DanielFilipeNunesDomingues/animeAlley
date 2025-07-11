using animeAlley.Models.ViewModels.EstatisticasDTO;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IEstatisticaService
    {
        // Estatísticas gerais completas
        Task<EstatisticaDTO> GetEstatisticasCompletasAsync();

        // Estatísticas resumidas
        Task<EstatisticaResumoDTO> GetEstatisticasResumoAsync();

        // Estatísticas por categorias
        Task<int> GetTotalShowsAsync();
        Task<int> GetTotalUtilizadoresAsync();
        Task<int> GetTotalGenerosAsync();
        Task<int> GetTotalStudiosAsync();
        Task<int> GetTotalAutoresAsync();
        Task<int> GetTotalPersonagensAsync();

        // Médias e análises
        Task<decimal> GetNotaMediaGeralAsync();

        // Top rankings
        Task<IEnumerable<GeneroPopularDTO>> GetGenerosPopularesAsync(int take = 10);
        Task<IEnumerable<ShowPopularDTO>> GetShowsPopularesAsync(int take = 10);
        Task<IEnumerable<StudioPopularDTO>> GetStudiosPopularesAsync(int take = 10);

        // Estatísticas por período
        Task<EstatisticaDTO> GetEstatisticasPorAnoAsync(int ano);
    }
}