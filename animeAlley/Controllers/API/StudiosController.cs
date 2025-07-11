using animeAlley.Models;
using animeAlley.Models.ViewModels.StudiosDTO;
using animeAlley.Services;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class StudiosController : ControllerBase
    {
        private readonly IStudioService _studioService;

        public StudiosController(IStudioService studioService)
        {
            _studioService = studioService;
        }

        // GET: api/Studios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudioDTO>>> GetStudios()
        {
            var studios = await _studioService.GetAllStudiosAsync();
            return Ok(studios);
        }

        // GET: api/Studios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudioDTO>> GetStudio(int id)
        {
            var studio = await _studioService.GetStudioByIdAsync(id);

            if (studio == null)
            {
                return NotFound($"Studio com ID {id} não encontrado.");
            }

            return Ok(studio);
        }

        // POST: api/Studios
        [HttpPost]
        public async Task<ActionResult<StudioDTO>> CreateStudio(StudioCreateUpdateDTO studioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se já existe um studio com o mesmo nome
            var existsName = await _studioService.StudioExistsByNameAsync(studioDto.Nome);
            if (existsName)
            {
                return Conflict($"Já existe um studio com o nome '{studioDto.Nome}'.");
            }

            var studio = await _studioService.CreateStudioAsync(studioDto);
            return CreatedAtAction(nameof(GetStudio), new { id = studio.Id }, studio);
        }

        // PUT: api/Studios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudio(int id, StudioCreateUpdateDTO studioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existsId = await _studioService.StudioExistsAsync(id);
            if (!existsId)
            {
                return NotFound($"Studio com ID {id} não encontrado.");
            }

            // Verificar se já existe outro studio com o mesmo nome
            var existingStudio = await _studioService.GetStudioByIdAsync(id);
            if (existingStudio != null && existingStudio.Nome != studioDto.Nome)
            {
                var existsName = await _studioService.StudioExistsByNameAsync(studioDto.Nome);
                if (existsName)
                {
                    return Conflict($"Já existe um studio com o nome '{studioDto.Nome}'.");
                }
            }

            var updatedStudio = await _studioService.UpdateStudioAsync(id, studioDto);
            if (updatedStudio == null)
            {
                return NotFound($"Studio com ID {id} não encontrado.");
            }

            return Ok(updatedStudio);
        }

        // DELETE: api/Studios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudio(int id)
        {
            var exists = await _studioService.StudioExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Studio com ID {id} não encontrado.");
            }

            var deleted = await _studioService.DeleteStudioAsync(id);
            if (!deleted)
            {
                return BadRequest("Não foi possível deletar o studio.");
            }

            return NoContent();
        }

        // GET: api/Studios/populares
        [HttpGet("populares")]
        public async Task<ActionResult<IEnumerable<StudioPopularDTO>>> GetStudiosPopulares([FromQuery] int take = 10)
        {
            if (take <= 0)
            {
                return BadRequest("O parâmetro 'take' deve ser maior que 0.");
            }

            var studiosPopulares = await _studioService.GetStudiosPopularesAsync(take);
            return Ok(studiosPopulares);
        }

        // GET: api/Studios/search?nome=studio
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudioDTO>>> SearchStudios([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var studios = await _studioService.SearchStudiosByNameAsync(nome);
            return Ok(studios);
        }

        // GET: api/Studios/ativos
        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<StudioDTO>>> GetStudiosAtivos()
        {
            var studios = await _studioService.GetStudiosAtivosAsync();
            return Ok(studios);
        }

        // GET: api/Studios/inativos
        [HttpGet("inativos")]
        public async Task<ActionResult<IEnumerable<StudioDTO>>> GetStudiosInativos()
        {
            var studios = await _studioService.GetStudiosInativosAsync();
            return Ok(studios);
        }

        // GET: api/Studios/fundado?dataInicio=2000-01-01&dataFim=2020-12-31
        [HttpGet("fundado")]
        public async Task<ActionResult<IEnumerable<StudioDTO>>> GetStudiosByFundado(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            if (!dataInicio.HasValue && !dataFim.HasValue)
            {
                return BadRequest("Pelo menos um dos parâmetros 'dataInicio' ou 'dataFim' deve ser informado.");
            }

            var studios = await _studioService.GetStudiosByFundadoRangeAsync(dataInicio, dataFim);
            return Ok(studios);
        }

        // GET: api/Studios/exists/5
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> CheckStudioExists(int id)
        {
            var exists = await _studioService.StudioExistsAsync(id);
            return Ok(exists);
        }

        // GET: api/Studios/exists/name?nome=studio
        [HttpGet("exists/name")]
        public async Task<ActionResult<bool>> CheckStudioExistsByName([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var exists = await _studioService.StudioExistsByNameAsync(nome);
            return Ok(exists);
        }

        // GET: api/Studios/estatisticas
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetTotalStudios()
        {
            var total = await _studioService.GetTotalStudiosAsync();
            return Ok(total);
        }
    }
}
