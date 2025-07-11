using animeAlley.Models;
using animeAlley.Models.ViewModels.GenerosDTO;
using animeAlley.Models.ViewModels.GenerosAPI;

namespace animeAlley.Services.Interfaces
{
    public interface IGeneroService
    {
        // Operações CRUD básicas
        Task<IEnumerable<GeneroDTO>> GetAllGenerosAsync();
        Task<GeneroDTO?> GetGeneroByIdAsync(int id);
        Task<GeneroDTO> CreateGeneroAsync(GeneroCreateUpdateDTO generoDto);
        Task<GeneroDTO?> UpdateGeneroAsync(int id, GeneroCreateUpdateDTO generoDto);
        Task<bool> DeleteGeneroAsync(int id);

        // Funcionalidades adicionais
        Task<IEnumerable<GeneroPopularDTO>> GetGenerosPopularesAsync(int take = 10);
        Task<IEnumerable<GeneroDTO>> SearchGenerosByNameAsync(string nome);
        Task<bool> GeneroExistsAsync(int id);
        Task<bool> GeneroExistsByNameAsync(string nome);
        Task<int> GetTotalGenerosAsync();
        Task<IEnumerable<GeneroDTO>> GetGenerosByShowCountRangeAsync(int minShows, int maxShows);
    }
}