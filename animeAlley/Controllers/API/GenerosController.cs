using animeAlley.Models;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.GenerosDTO;
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
    public class GenerosController : ControllerBase
    {
        private readonly IGeneroService _generoService;

        public GenerosController(IGeneroService generoService)
        {
            _generoService = generoService;
        }

        /// <summary>
        /// Obtém todos os gêneros
        /// </summary>
        /// <returns>Lista de gêneros</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeneroDTO>>> GetAllGeneros()
        {
            var generos = await _generoService.GetAllGenerosAsync();
            return Ok(generos);
        }

        /// <summary>
        /// Obtém um gênero por ID
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Gênero encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneroDTO>> GetGeneroById(int id)
        {
            var genero = await _generoService.GetGeneroByIdAsync(id);
            if (genero == null)
            {
                return NotFound($"Gênero com ID {id} não encontrado.");
            }
            return Ok(genero);
        }

        /// <summary>
        /// Cria um novo gênero
        /// </summary>
        /// <param name="generoDto">Dados do gênero a ser criado</param>
        /// <returns>Gênero criado</returns>
        [HttpPost]
        public async Task<ActionResult<GeneroDTO>> CreateGenero([FromBody] GeneroCreateUpdateDTO generoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se já existe um gênero com o mesmo nome
            if (await _generoService.GeneroExistsByNameAsync(generoDto.Nome))
            {
                return Conflict($"Já existe um gênero com o nome '{generoDto.Nome}'.");
            }

            var genero = await _generoService.CreateGeneroAsync(generoDto);
            return CreatedAtAction(nameof(GetGeneroById), new { id = genero.Id }, genero);
        }

        /// <summary>
        /// Atualiza um gênero existente
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <param name="generoDto">Dados atualizados do gênero</param>
        /// <returns>Gênero atualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<GeneroDTO>> UpdateGenero(int id, [FromBody] GeneroCreateUpdateDTO generoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se existe outro gênero com o mesmo nome
            var existingGenero = await _generoService.GetGeneroByIdAsync(id);
            if (existingGenero == null)
            {
                return NotFound($"Gênero com ID {id} não encontrado.");
            }

            if (await _generoService.GeneroExistsByNameAsync(generoDto.Nome) &&
                existingGenero.Nome != generoDto.Nome)
            {
                return Conflict($"Já existe um gênero com o nome '{generoDto.Nome}'.");
            }

            var genero = await _generoService.UpdateGeneroAsync(id, generoDto);
            if (genero == null)
            {
                return NotFound($"Gênero com ID {id} não encontrado.");
            }

            return Ok(genero);
        }

        /// <summary>
        /// Remove um gênero
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGenero(int id)
        {
            var deleted = await _generoService.DeleteGeneroAsync(id);
            if (!deleted)
            {
                return NotFound($"Gênero com ID {id} não encontrado.");
            }

            return NoContent();
        }

        /// <summary>
        /// Obtém os gêneros mais populares
        /// </summary>
        /// <param name="take">Quantidade de gêneros a retornar (padrão: 10)</param>
        /// <returns>Lista dos gêneros mais populares</returns>
        [HttpGet("populares")]
        public async Task<ActionResult<IEnumerable<GeneroPopularDTO>>> GetGenerosPopulares([FromQuery] int take = 10)
        {
            if (take <= 0)
            {
                return BadRequest("O parâmetro 'take' deve ser maior que zero.");
            }

            var generos = await _generoService.GetGenerosPopularesAsync(take);
            return Ok(generos);
        }

        /// <summary>
        /// Busca gêneros por nome
        /// </summary>
        /// <param name="nome">Nome ou parte do nome do gênero</param>
        /// <returns>Lista de gêneros encontrados</returns>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<GeneroDTO>>> SearchGenerosByName([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var generos = await _generoService.SearchGenerosByNameAsync(nome);
            return Ok(generos);
        }

        /// <summary>
        /// Obtém gêneros por faixa de quantidade de shows
        /// </summary>
        /// <param name="minShows">Quantidade mínima de shows</param>
        /// <param name="maxShows">Quantidade máxima de shows</param>
        /// <returns>Lista de gêneros na faixa especificada</returns>
        [HttpGet("por-quantidade-shows")]
        public async Task<ActionResult<IEnumerable<GeneroDTO>>> GetGenerosByShowCountRange(
            [FromQuery] int minShows = 0,
            [FromQuery] int maxShows = int.MaxValue)
        {
            if (minShows < 0 || maxShows < 0 || minShows > maxShows)
            {
                return BadRequest("Parâmetros inválidos para a faixa de quantidade de shows.");
            }

            var generos = await _generoService.GetGenerosByShowCountRangeAsync(minShows, maxShows);
            return Ok(generos);
        }

        /// <summary>
        /// Verifica se um gênero existe
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Booleano indicando se existe</returns>
        [HttpGet("{id}/existe")]
        public async Task<ActionResult<bool>> GeneroExists(int id)
        {
            var exists = await _generoService.GeneroExistsAsync(id);
            return Ok(exists);
        }

        /// <summary>
        /// Obtém estatísticas dos gêneros
        /// </summary>
        /// <returns>Total de gêneros</returns>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetEstatisticas()
        {
            var totalGeneros = await _generoService.GetTotalGenerosAsync();

            return Ok(new
            {
                TotalGeneros = totalGeneros
            });
        }
    }
}
