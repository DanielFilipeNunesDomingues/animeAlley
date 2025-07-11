using animeAlley.Models;
using animeAlley.Models.ViewModels.PersonagensDTO;

namespace animeAlley.Services.Interfaces
{
    public interface IPersonagemService
    {
        // Operações CRUD básicas
        Task<IEnumerable<PersonagemDTO>> GetAllPersonagensAsync();
        Task<PersonagemDTO?> GetPersonagemByIdAsync(int id);
        Task<PersonagemDTO> CreatePersonagemAsync(PersonagemCreateUpdateDTO personagemDto);
        Task<PersonagemDTO?> UpdatePersonagemAsync(int id, PersonagemCreateUpdateDTO personagemDto);
        Task<bool> DeletePersonagemAsync(int id);

        // Funcionalidades adicionais
        Task<IEnumerable<PersonagemResumoDTO>> GetPersonagensResumoAsync();
        Task<IEnumerable<PersonagemDTO>> SearchPersonagensByNameAsync(string nome);
        Task<IEnumerable<PersonagemDTO>> GetPersonagensByTipoAsync(string tipoPersonagem);
        Task<IEnumerable<PersonagemDTO>> GetPersonagensBySexualidadeAsync(string sexualidade);
        Task<IEnumerable<PersonagemDTO>> GetPersonagensByIdadeRangeAsync(int idadeMin, int idadeMax);
        Task<IEnumerable<PersonagemDTO>> GetPersonagensByShowIdAsync(int showId);
        Task<IEnumerable<PersonagemDTO>> GetPersonagensProtagonistasAsync();
        Task<IEnumerable<PersonagemDTO>> GetPersonagensAntagonistasAsync();
        Task<IEnumerable<PersonagemDTO>> GetPersonagensSecundariosAsync();
        Task<IEnumerable<PersonagemDTO>> GetPersonagensFigurantesAsync();
        Task<bool> PersonagemExistsAsync(int id);
        Task<bool> PersonagemExistsByNameAsync(string nome);
        Task<object> GetTotalPersonagensAsync();
    }
}