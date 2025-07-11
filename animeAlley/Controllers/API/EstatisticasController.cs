using animeAlley.Models.ViewModels.EstatisticasDTO;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;
using animeAlley.Services;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace animeAlley.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EstatisticasController : ControllerBase
    {
        private readonly IEstatisticaService _estatisticaService;

        public EstatisticasController(IEstatisticaService estatisticaService)
        {
            _estatisticaService = estatisticaService;
        }

        /// <summary>
        /// Obtém estatísticas completas da aplicação
        /// </summary>
        /// <returns>Estatísticas completas</returns>
        [HttpGet("completas")]
        public async Task<ActionResult<EstatisticaDTO>> GetEstatisticasCompletas()
        {
            try
            {
                var estatisticas = await _estatisticaService.GetEstatisticasCompletasAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas de tendências
        /// </summary>
        /// <returns>Tendências baseadas nos dados mais recentes</returns>
        [HttpGet("tendencias")]
        public async Task<ActionResult<object>> GetTendencias()
        {
            try
            {
                var anoAtual = DateTime.Now.Year;
                var mesAtual = DateTime.Now.Month;

                var estatisticasAnoAtual = await _estatisticaService.GetEstatisticasPorAnoAsync(anoAtual);
                var generosPopulares = await _estatisticaService.GetGenerosPopularesAsync(5);
                var showsPopulares = await _estatisticaService.GetShowsPopularesAsync(5);

                var tendencias = new
                {
                    ano = anoAtual,
                    mes = mesAtual,
                    estatisticasAno = estatisticasAnoAtual,
                    topGeneros = generosPopulares,
                    topShows = showsPopulares
                };

                return Ok(tendencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém resumo das estatísticas da aplicação
        /// </summary>
        /// <returns>Resumo das estatísticas</returns>
        [HttpGet("resumo")]
        public async Task<ActionResult<EstatisticaResumoDTO>> GetEstatisticasResumo()
        {
            try
            {
                var estatisticas = await _estatisticaService.GetEstatisticasResumoAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de shows
        /// </summary>
        /// <returns>Total de shows</returns>
        [HttpGet("total-shows")]
        public async Task<ActionResult<int>> GetTotalShows()
        {
            try
            {
                var total = await _estatisticaService.GetTotalShowsAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de utilizadores
        /// </summary>
        /// <returns>Total de utilizadores</returns>
        [HttpGet("total-utilizadores")]
        public async Task<ActionResult<int>> GetTotalUtilizadores()
        {
            try
            {
                var total = await _estatisticaService.GetTotalUtilizadoresAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de géneros
        /// </summary>
        /// <returns>Total de géneros</returns>
        [HttpGet("total-generos")]
        public async Task<ActionResult<int>> GetTotalGeneros()
        {
            try
            {
                var total = await _estatisticaService.GetTotalGenerosAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de studios
        /// </summary>
        /// <returns>Total de studios</returns>
        [HttpGet("total-studios")]
        public async Task<ActionResult<int>> GetTotalStudios()
        {
            try
            {
                var total = await _estatisticaService.GetTotalStudiosAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de autores
        /// </summary>
        /// <returns>Total de autores</returns>
        [HttpGet("total-autores")]
        public async Task<ActionResult<int>> GetTotalAutores()
        {
            try
            {
                var total = await _estatisticaService.GetTotalAutoresAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o total de personagens
        /// </summary>
        /// <returns>Total de personagens</returns>
        [HttpGet("total-personagens")]
        public async Task<ActionResult<int>> GetTotalPersonagens()
        {
            try
            {
                var total = await _estatisticaService.GetTotalPersonagensAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a nota média geral
        /// </summary>
        /// <returns>Nota média geral</returns>
        [HttpGet("nota-media-geral")]
        public async Task<ActionResult<decimal>> GetNotaMediaGeral()
        {
            try
            {
                var media = await _estatisticaService.GetNotaMediaGeralAsync();
                return Ok(new { notaMedia = media });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os géneros mais populares
        /// </summary>
        /// <param name="take">Número de géneros a retornar (padrão: 10)</param>
        /// <returns>Lista de géneros populares</returns>
        [HttpGet("generos-populares")]
        public async Task<ActionResult<IEnumerable<GeneroPopularDTO>>> GetGenerosPopulares([FromQuery] int take = 10)
        {
            try
            {
                var generos = await _estatisticaService.GetGenerosPopularesAsync(take);
                return Ok(generos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os shows mais populares
        /// </summary>
        /// <param name="take">Número de shows a retornar (padrão: 10)</param>
        /// <returns>Lista de shows populares</returns>
        [HttpGet("shows-populares")]
        public async Task<ActionResult<IEnumerable<ShowPopularDTO>>> GetShowsPopulares([FromQuery] int take = 10)
        {
            try
            {
                var shows = await _estatisticaService.GetShowsPopularesAsync(take);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os studios mais populares
        /// </summary>
        /// <param name="take">Número de studios a retornar (padrão: 10)</param>
        /// <returns>Lista de studios populares</returns>
        [HttpGet("studios-populares")]
        public async Task<ActionResult<IEnumerable<StudioPopularDTO>>> GetStudiosPopulares([FromQuery] int take = 10)
        {
            try
            {
                var studios = await _estatisticaService.GetStudiosPopularesAsync(take);
                return Ok(studios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas por ano
        /// </summary>
        /// <param name="ano">Ano para análise</param>
        /// <returns>Estatísticas do ano especificado</returns>
        [HttpGet("por-ano/{ano}")]
        public async Task<ActionResult<EstatisticaDTO>> GetEstatisticasPorAno(int ano)
        {
            try
            {
                var estatisticas = await _estatisticaService.GetEstatisticasPorAnoAsync(ano);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }
    }
}

