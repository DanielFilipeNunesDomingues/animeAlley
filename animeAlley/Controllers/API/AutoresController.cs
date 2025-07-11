using animeAlley.Models.ViewModels.AutoresDTO;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace animeAlley.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        /// <summary>
        /// Obtém todos os autores
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAllAutores()
        {
            var autores = await _autorService.GetAllAutoresAsync();
            return Ok(autores);
        }

        /// <summary>
        /// Obtém um autor por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AutorDTO>> GetAutorById(int id)
        {
            var autor = await _autorService.GetAutorByIdAsync(id);
            if (autor == null)
                return NotFound($"Autor com ID {id} não encontrado.");

            return Ok(autor);
        }

        /// <summary>
        /// Cria um novo autor
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AutorDTO>> CreateAutor([FromBody] AutorCreateUpdateDTO autorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se já existe um autor com o mesmo nome
            if (await _autorService.AutorExistsByNameAsync(autorDto.Nome))
                return Conflict($"Já existe um autor com o nome '{autorDto.Nome}'.");

            var autor = await _autorService.CreateAutorAsync(autorDto);
            return CreatedAtAction(nameof(GetAutorById), new { id = autor.Id }, autor);
        }

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AutorDTO>> UpdateAutor(int id, [FromBody] AutorCreateUpdateDTO autorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var autor = await _autorService.UpdateAutorAsync(id, autorDto);
            if (autor == null)
                return NotFound($"Autor com ID {id} não encontrado.");

            return Ok(autor);
        }

        /// <summary>
        /// Remove um autor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAutor(int id)
        {
            var success = await _autorService.DeleteAutorAsync(id);
            if (!success)
                return NotFound($"Autor com ID {id} não encontrado.");

            return NoContent();
        }

        /// <summary>
        /// Obtém os autores mais populares
        /// </summary>
        [HttpGet("populares")]
        public async Task<ActionResult<IEnumerable<AutorPopularDTO>>> GetAutoresPopulares([FromQuery] int take = 10)
        {
            var autores = await _autorService.GetAutoresPopularesAsync(take);
            return Ok(autores);
        }

        /// <summary>
        /// Busca autores por nome
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> SearchAutoresByName([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("O parâmetro 'nome' é obrigatório.");

            var autores = await _autorService.SearchAutoresByNameAsync(nome);
            return Ok(autores);
        }

        /// <summary>
        /// Obtém autores por faixa etária
        /// </summary>
        [HttpGet("idade")]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAutoresByIdadeRange(
            [FromQuery] int idadeMin = 0,
            [FromQuery] int idadeMax = 150)
        {
            if (idadeMin < 0 || idadeMax < 0 || idadeMin > idadeMax)
                return BadRequest("Faixa etária inválida.");

            var autores = await _autorService.GetAutoresByIdadeRangeAsync(idadeMin, idadeMax);
            return Ok(autores);
        }

        /// <summary>
        /// Obtém autores por sexo
        /// </summary>
        [HttpGet("sexo/{sexo}")]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAutoresBySexo(string sexo)
        {
            if (string.IsNullOrWhiteSpace(sexo))
                return BadRequest("O parâmetro 'sexo' é obrigatório.");

            var autores = await _autorService.GetAutoresBySexoAsync(sexo);
            return Ok(autores);
        }

        /// <summary>
        /// Verifica se um autor existe
        /// </summary>
        [HttpHead("{id}")]
        public async Task<ActionResult> CheckAutorExists(int id)
        {
            var exists = await _autorService.AutorExistsAsync(id);
            return exists ? Ok() : NotFound();
        }

        /// <summary>
        /// Obtém estatísticas dos autores
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetEstatisticas()
        {
            var totalAutores = await _autorService.GetTotalAutoresAsync();
            var autoresPopulares = await _autorService.GetAutoresPopularesAsync(5);

            var estatisticas = new
            {
                TotalAutores = totalAutores,
                Top5Populares = autoresPopulares
            };

            return Ok(estatisticas);
        }
    }
}