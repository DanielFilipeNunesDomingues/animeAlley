using AutoMapper;
using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.PersonagensDTO;
using animeAlley.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class PersonagemService : IPersonagemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PersonagemService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Operações CRUD básicas
        public async Task<IEnumerable<PersonagemDTO>> GetAllPersonagensAsync()
        {
            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<PersonagemDTO?> GetPersonagemByIdAsync(int id)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            return personagem == null ? null : _mapper.Map<PersonagemDTO>(personagem);
        }

        public async Task<PersonagemDTO> CreatePersonagemAsync(PersonagemCreateUpdateDTO personagemDto)
        {
            var personagem = _mapper.Map<Personagem>(personagemDto);

            // Adicionar shows relacionados
            if (personagemDto.ShowIds.Any())
            {
                var shows = await _context.Shows
                    .Where(s => personagemDto.ShowIds.Contains(s.Id))
                    .ToListAsync();

                personagem.Shows = shows;
            }

            _context.Personagens.Add(personagem);
            await _context.SaveChangesAsync();

            // Recarregar com includes para o mapeamento correto
            var createdPersonagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstAsync(p => p.Id == personagem.Id);

            return _mapper.Map<PersonagemDTO>(createdPersonagem);
        }

        public async Task<PersonagemDTO?> UpdatePersonagemAsync(int id, PersonagemCreateUpdateDTO personagemDto)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null)
                return null;

            // Mapear as propriedades do DTO para a entidade existente
            _mapper.Map(personagemDto, personagem);

            // Atualizar shows relacionados
            personagem.Shows.Clear();
            if (personagemDto.ShowIds.Any())
            {
                var shows = await _context.Shows
                    .Where(s => personagemDto.ShowIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var show in shows)
                {
                    personagem.Shows.Add(show);
                }
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<PersonagemDTO>(personagem);
        }

        public async Task<bool> DeletePersonagemAsync(int id)
        {
            var personagem = await _context.Personagens.FindAsync(id);
            if (personagem == null)
                return false;

            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();
            return true;
        }

        // Funcionalidades adicionais
        public async Task<IEnumerable<PersonagemResumoDTO>> GetPersonagensResumoAsync()
        {
            var personagens = await _context.Personagens.ToListAsync();
            return _mapper.Map<IEnumerable<PersonagemResumoDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> SearchPersonagensByNameAsync(string nome)
        {
            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .Where(p => p.Nome.Contains(nome))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensByTipoAsync(string tipoPersonagem)
        {
            if (!Enum.TryParse<TiposPersonagem>(tipoPersonagem, out var tipoEnum))
                return new List<PersonagemDTO>();

            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .Where(p => p.TipoPersonagem == tipoEnum)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensBySexualidadeAsync(string sexualidade)
        {
            if (!Enum.TryParse<Sexualidade>(sexualidade, out var sexualidadeEnum))
                return new List<PersonagemDTO>();

            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .Where(p => p.PersonagemSexualidade == sexualidadeEnum)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensByIdadeRangeAsync(int idadeMin, int idadeMax)
        {
            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .Where(p => p.Idade >= idadeMin && p.Idade <= idadeMax)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensByShowIdAsync(int showId)
        {
            var personagens = await _context.Personagens
                .Include(p => p.Shows)
                .Where(p => p.Shows.Any(s => s.Id == showId))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PersonagemDTO>>(personagens);
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensProtagonistasAsync()
        {
            return await GetPersonagensByTipoAsync("Protagonista");
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensAntagonistasAsync()
        {
            return await GetPersonagensByTipoAsync("Antagonista");
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensSecundariosAsync()
        {
            return await GetPersonagensByTipoAsync("Secundario");
        }

        public async Task<IEnumerable<PersonagemDTO>> GetPersonagensFigurantesAsync()
        {
            return await GetPersonagensByTipoAsync("Figurante");
        }

        public async Task<bool> PersonagemExistsAsync(int id)
        {
            return await _context.Personagens.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> PersonagemExistsByNameAsync(string nome)
        {
            return await _context.Personagens.AnyAsync(p => p.Nome == nome);
        }

        public async Task<object> GetTotalPersonagensAsync()
        {
            var totalPersonagens = await _context.Personagens.CountAsync();
            var idadeMedia = await _context.Personagens.Where(p => p.Idade > 0).AverageAsync(p => p.Idade);
            return new
            {
                TotalPersonagens = totalPersonagens,
                IdadeMedia = idadeMedia
            };
        }
    }
}