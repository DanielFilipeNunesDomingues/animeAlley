using animeAlley.Models;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowsService _showsService; // Changed from ShowsService to IShowsService
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowsService showsService, ILogger<ShowsController> logger) // Changed parameter type
        {
            _showsService = showsService;
            _logger = logger;
        }

        /// <summary>
        /// Busca shows com filtros e paginação
        /// </summary>
        /// <param name="filtro">Filtros para busca</param>
        /// <returns>Lista paginada de shows</returns>
        [HttpGet]
        public async Task<IActionResult> BuscarShows([FromQuery] ShowFiltroDTO filtro)
        {
            try
            {
                var resultado = await _showsService.BuscarShowsAsync(filtro);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca um show específico por ID
        /// </summary>
        /// <param name="id">ID do show</param>
        /// <returns>Detalhes do show</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterShowPorId(int id)
        {
            try
            {
                var show = await _showsService.ObterShowPorIdAsync(id);
                if (show == null)
                {
                    return NotFound(new { message = "Show não encontrado" });
                }
                return Ok(show);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar show com ID {ShowId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Cria um novo show
        /// </summary>
        /// <param name="showDto">Dados para criação do show</param>
        /// <returns>Show criado</returns>
        [HttpPost]
        public async Task<IActionResult> CriarShow([FromBody] ShowCreateUpdateDTO showDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var showCriado = await _showsService.CriarShowAsync(showDto);
                return CreatedAtAction(nameof(ObterShowPorId), new { id = showCriado.Id }, showCriado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar show");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza um show existente
        /// </summary>
        /// <param name="id">ID do show a ser atualizado</param>
        /// <param name="showDto">Dados para atualização</param>
        /// <returns>Show atualizado</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarShow(int id, [FromBody] ShowCreateUpdateDTO showDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var showAtualizado = await _showsService.AtualizarShowAsync(id, showDto);
                if (showAtualizado == null)
                {
                    return NotFound(new { message = "Show não encontrado" });
                }
                return Ok(showAtualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar show com ID {ShowId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Remove um show
        /// </summary>
        /// <param name="id">ID do show a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverShow(int id)
        {
            try
            {
                var removido = await _showsService.RemoverShowAsync(id);
                if (!removido)
                {
                    return NotFound(new { message = "Show não encontrado" });
                }
                return Ok(new { message = "Show removido com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover show com ID {ShowId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca shows mais populares
        /// </summary>
        /// <param name="limite">Número máximo de shows a retornar</param>
        /// <returns>Lista de shows populares</returns>
        [HttpGet("populares")]
        public async Task<IActionResult> ObterShowsPopulares([FromQuery] int limite = 10)
        {
            try
            {
                var showsPopulares = await _showsService.ObterShowsPopularesAsync(limite);
                return Ok(showsPopulares);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows populares");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca resumo de shows com filtros básicos
        /// </summary>
        /// <param name="search">Termo de busca</param>
        /// <param name="generoIds">IDs dos gêneros</param>
        /// <param name="limite">Número máximo de shows</param>
        /// <returns>Lista de resumos de shows</returns>
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumoShows([FromQuery] string? search = null, [FromQuery] List<int>? generoIds = null, [FromQuery] int limite = 20)
        {
            try
            {
                var resumos = await _showsService.ObterResumoShowsAsync(search, generoIds, limite);
                return Ok(resumos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar resumo de shows");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Gera relatório de shows
        /// </summary>
        /// <param name="inicio">Data de início do período</param>
        /// <param name="fim">Data de fim do período</param>
        /// <returns>Relatório de shows</returns>
        [EnableRateLimiting("ApiPolicy")]
        [HttpGet("relatorio")]
        public async Task<IActionResult> GerarRelatorioShows( [FromQuery] int? inicio = null, [FromQuery] int? fim = null)
        {
            try
            {
                var relatorio = await _showsService.GerarRelatorioShowsAsync(inicio, fim);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de shows");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca shows por autor
        /// </summary>
        /// <param name="autorId">ID do autor</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de shows do autor</returns>
        [HttpGet("autor/{autorId}")]
        public async Task<IActionResult> ObterShowsPorAutor(int autorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var shows = await _showsService.ObterShowsPorAutorAsync(autorId, page, pageSize);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows por autor {AutorId}", autorId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca shows por estúdio
        /// </summary>
        /// <param name="studioId">ID do estúdio</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de shows do estúdio</returns>
        [HttpGet("studio/{studioId}")]
        public async Task<IActionResult> ObterShowsPorStudio(int studioId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var shows = await _showsService.ObterShowsPorStudioAsync(studioId, page, pageSize);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows por estúdio {StudioId}", studioId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca shows por gênero
        /// </summary>
        /// <param name="generoId">ID do gênero</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de shows do gênero</returns>
        [HttpGet("genero/{generoId}")]
        public async Task<IActionResult> ObterShowsPorGenero(int generoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var shows = await _showsService.ObterShowsPorGeneroAsync(generoId, page, pageSize);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows por gênero {GeneroId}", generoId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca shows por ano
        /// </summary>
        /// <param name="ano">Ano de lançamento</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de shows do ano</returns>
        [HttpGet("ano/{ano:int:min(1900):max(2050)}")]
        public async Task<IActionResult> ObterShowsPorAno(int ano, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                if (ano < 1900 || ano > DateTime.Now.Year + 10)
                {
                    return BadRequest(new { message = "Ano inválido" });
                }

                var shows = await _showsService.ObterShowsPorAnoAsync(ano, page, pageSize);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar shows por ano {Ano}", ano);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca estatísticas gerais dos shows
        /// </summary>
        /// <returns>Estatísticas dos shows</returns>
        [HttpGet("estatisticas")]
        public async Task<IActionResult> ObterEstatisticasShows()
        {
            try
            {
                var estatisticas = await _showsService.ObterEstatisticasShowsAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas de shows");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}