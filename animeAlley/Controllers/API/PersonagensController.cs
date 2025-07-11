using animeAlley.Models;
using animeAlley.Models.ViewModels.PersonagensDTO;
using animeAlley.Services;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PersonagensController : ControllerBase
    {
        private readonly IPersonagemService _personagemService;

        public PersonagensController(IPersonagemService personagemService)
        {
            _personagemService = personagemService;
        }

        // GET: api/Personagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagens()
        {
            var personagens = await _personagemService.GetAllPersonagensAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonagemDTO>> GetPersonagem(int id)
        {
            var personagem = await _personagemService.GetPersonagemByIdAsync(id);

            if (personagem == null)
            {
                return NotFound($"Personagem com ID {id} não encontrado.");
            }

            return Ok(personagem);
        }

        // POST: api/Personagens
        [HttpPost]
        public async Task<ActionResult<PersonagemDTO>> CreatePersonagem(PersonagemCreateUpdateDTO personagemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se já existe um personagem com o mesmo nome
            var existsName = await _personagemService.PersonagemExistsByNameAsync(personagemDto.Nome);
            if (existsName)
            {
                return Conflict($"Já existe um personagem com o nome '{personagemDto.Nome}'.");
            }

            var personagem = await _personagemService.CreatePersonagemAsync(personagemDto);
            return CreatedAtAction(nameof(GetPersonagem), new { id = personagem.Id }, personagem);
        }

        // PUT: api/Personagens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonagem(int id, PersonagemCreateUpdateDTO personagemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existsId = await _personagemService.PersonagemExistsAsync(id);
            if (!existsId)
            {
                return NotFound($"Personagem com ID {id} não encontrado.");
            }

            // Verificar se já existe outro personagem com o mesmo nome
            var existingPersonagem = await _personagemService.GetPersonagemByIdAsync(id);
            if (existingPersonagem != null && existingPersonagem.Nome != personagemDto.Nome)
            {
                var existsName = await _personagemService.PersonagemExistsByNameAsync(personagemDto.Nome);
                if (existsName)
                {
                    return Conflict($"Já existe um personagem com o nome '{personagemDto.Nome}'.");
                }
            }

            var updatedPersonagem = await _personagemService.UpdatePersonagemAsync(id, personagemDto);
            if (updatedPersonagem == null)
            {
                return NotFound($"Personagem com ID {id} não encontrado.");
            }

            return Ok(updatedPersonagem);
        }

        // DELETE: api/Personagens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonagem(int id)
        {
            var exists = await _personagemService.PersonagemExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Personagem com ID {id} não encontrado.");
            }

            var deleted = await _personagemService.DeletePersonagemAsync(id);
            if (!deleted)
            {
                return BadRequest("Não foi possível deletar o personagem.");
            }

            return NoContent();
        }

        // GET: api/Personagens/resumo
        [HttpGet("resumo")]
        public async Task<ActionResult<IEnumerable<PersonagemResumoDTO>>> GetPersonagensResumo()
        {
            var personagens = await _personagemService.GetPersonagensResumoAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/search?nome=nome
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> SearchPersonagens([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var personagens = await _personagemService.SearchPersonagensByNameAsync(nome);
            return Ok(personagens);
        }

        // GET: api/Personagens/tipo/Protagonista
        [HttpGet("tipo/{tipoPersonagem}")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensByTipo(string tipoPersonagem)
        {
            if (string.IsNullOrWhiteSpace(tipoPersonagem))
            {
                return BadRequest("O parâmetro 'tipoPersonagem' é obrigatório.");
            }

            var personagens = await _personagemService.GetPersonagensByTipoAsync(tipoPersonagem);
            return Ok(personagens);
        }

        // GET: api/Personagens/protagonistas
        [HttpGet("protagonistas")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensProtagonistas()
        {
            var personagens = await _personagemService.GetPersonagensProtagonistasAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/antagonistas
        [HttpGet("antagonistas")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensAntagonistas()
        {
            var personagens = await _personagemService.GetPersonagensAntagonistasAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/secundarios
        [HttpGet("secundarios")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensSecundarios()
        {
            var personagens = await _personagemService.GetPersonagensSecundariosAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/figurantes
        [HttpGet("figurantes")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensFigurantes()
        {
            var personagens = await _personagemService.GetPersonagensFigurantesAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/sexualidade/Masculino
        [HttpGet("sexualidade/{sexualidade}")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensBySexualidade(string sexualidade)
        {
            if (string.IsNullOrWhiteSpace(sexualidade))
            {
                return BadRequest("O parâmetro 'sexualidade' é obrigatório.");
            }

            var personagens = await _personagemService.GetPersonagensBySexualidadeAsync(sexualidade);
            return Ok(personagens);
        }

        // GET: api/Personagens/idade?min=18&max=30
        [HttpGet("idade")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensByIdade(
            [FromQuery] int min = 0,
            [FromQuery] int max = 1000)
        {
            if (min < 0 || max < 0 || min > max)
            {
                return BadRequest("Os parâmetros de idade devem ser válidos e min deve ser menor ou igual a max.");
            }

            var personagens = await _personagemService.GetPersonagensByIdadeRangeAsync(min, max);
            return Ok(personagens);
        }

        // GET: api/Personagens/show/5
        [HttpGet("show/{showId}")]
        public async Task<ActionResult<IEnumerable<PersonagemDTO>>> GetPersonagensByShow(int showId)
        {
            var personagens = await _personagemService.GetPersonagensByShowIdAsync(showId);
            return Ok(personagens);
        }

        // GET: api/Personagens/exists/5
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> CheckPersonagemExists(int id)
        {
            var exists = await _personagemService.PersonagemExistsAsync(id);
            return Ok(exists);
        }

        // GET: api/Personagens/exists/name?nome=nome
        [HttpGet("exists/name")]
        public async Task<ActionResult<bool>> CheckPersonagemExistsByName([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var exists = await _personagemService.PersonagemExistsByNameAsync(nome);
            return Ok(exists);
        }

        // GET: api/Personagens/estatisticas
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetTotalPersonagens()
        {
            var total = await _personagemService.GetTotalPersonagensAsync();
            return Ok(total);
        }
    }
}
