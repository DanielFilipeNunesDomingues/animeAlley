using animeAlley.Models;
using animeAlley.Models.ViewModels.StudiosDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IStudioService
    {
        // Operações CRUD básicas
        Task<IEnumerable<StudioDTO>> GetAllStudiosAsync();
        Task<StudioDTO?> GetStudioByIdAsync(int id);
        Task<StudioDTO> CreateStudioAsync(StudioCreateUpdateDTO studioDto);
        Task<StudioDTO?> UpdateStudioAsync(int id, StudioCreateUpdateDTO studioDto);
        Task<bool> DeleteStudioAsync(int id);

        // Funcionalidades adicionais
        Task<IEnumerable<StudioPopularDTO>> GetStudiosPopularesAsync(int take = 10);
        Task<IEnumerable<StudioDTO>> SearchStudiosByNameAsync(string nome);
        Task<IEnumerable<StudioDTO>> GetStudiosByStatusAsync(string status);
        Task<IEnumerable<StudioDTO>> GetStudiosByFundadoRangeAsync(DateTime? dataInicio, DateTime? dataFim);
        Task<IEnumerable<StudioDTO>> GetStudiosAtivosAsync();
        Task<IEnumerable<StudioDTO>> GetStudiosInativosAsync();
        Task<bool> StudioExistsAsync(int id);
        Task<bool> StudioExistsByNameAsync(string nome);
        Task<object> GetTotalStudiosAsync();
    }
}