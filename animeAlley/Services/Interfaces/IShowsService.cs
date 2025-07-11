using animeAlley.Models.ViewModels.ShowsDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IShowsService
    {
        // Operações CRUD básicas
        Task<IEnumerable<ShowResumoDTO>> BuscarShowsAsync(ShowFiltroDTO filtro);
        Task<ShowDetalheDTO?> ObterShowPorIdAsync(int id);
        Task<ShowDetalheDTO> CriarShowAsync(ShowCreateUpdateDTO showDto);
        Task<ShowDetalheDTO?> AtualizarShowAsync(int id, ShowCreateUpdateDTO showDto);
        Task<bool> RemoverShowAsync(int id);

        // Consultas especializadas
        Task<IEnumerable<ShowPopularDTO>> ObterShowsPopularesAsync(int limite);
        Task<IEnumerable<ShowResumoDTO>> ObterResumoShowsAsync(string? search, List<int>? generoIds, int limite);
        Task<RelatorioShowsDTO> GerarRelatorioShowsAsync(int? inicio, int? fim);

        // Consultas por critérios específicos
        Task<IEnumerable<ShowResumoDTO>> ObterShowsPorAutorAsync(int autorId, int page, int pageSize);
        Task<IEnumerable<ShowResumoDTO>> ObterShowsPorStudioAsync(int studioId, int page, int pageSize);
        Task<IEnumerable<ShowResumoDTO>> ObterShowsPorGeneroAsync(int generoId, int page, int pageSize);
        Task<IEnumerable<ShowResumoDTO>> ObterShowsPorAnoAsync(int ano, int page, int pageSize);

        // Funcionalidades adicionais
        Task<object> ObterEstatisticasShowsAsync();
    }
}
