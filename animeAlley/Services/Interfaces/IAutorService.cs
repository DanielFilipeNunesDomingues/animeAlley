using animeAlley.Models;
using animeAlley.Models.ViewModels.AutoresDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IAutorService
    {
        // Operações CRUD básicas
        Task<IEnumerable<AutorDTO>> GetAllAutoresAsync();
        Task<AutorDTO?> GetAutorByIdAsync(int id);
        Task<AutorDTO> CreateAutorAsync(AutorCreateUpdateDTO autorDto);
        Task<AutorDTO?> UpdateAutorAsync(int id, AutorCreateUpdateDTO autorDto);
        Task<bool> DeleteAutorAsync(int id);

        // Funcionalidades adicionais
        Task<IEnumerable<AutorPopularDTO>> GetAutoresPopularesAsync(int take = 10);
        Task<IEnumerable<AutorDTO>> SearchAutoresByNameAsync(string nome);
        Task<IEnumerable<AutorDTO>> GetAutoresByIdadeRangeAsync(int idadeMin, int idadeMax);
        Task<IEnumerable<AutorDTO>> GetAutoresBySexoAsync(string sexo);
        Task<bool> AutorExistsAsync(int id);
        Task<bool> AutorExistsByNameAsync(string nome);
        Task<int> GetTotalAutoresAsync();
    }
}