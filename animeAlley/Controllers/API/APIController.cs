using animeAlley.Data;
using animeAlley.DTOs;
using animeAlley.Models;
using animeAlley.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers.Api
{

    // ================== CONTROLLER PRINCIPAL DE SHOWS (ANIMES/MANGAS) ==================
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        // GET: api/shows
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowResumoDto>>>> GetShows(
            [FromQuery] string? search = null,
            [FromQuery] string? generoIds = null,
            [FromQuery] string? studioIds = null,
            [FromQuery] string? autorIds = null,
            [FromQuery] string? tipo = null,
            [FromQuery] string? status = null,
            [FromQuery] decimal? notaMinima = null,
            [FromQuery] decimal? notaMaxima = null,
            [FromQuery] int? anoInicio = null,
            [FromQuery] int? anoFim = null,
            [FromQuery] string? fonte = null,
            [FromQuery] string ordenarPor = "nome",
            [FromQuery] string direcao = "ASC",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filtro = new ShowFiltroDto
                {
                    Search = search,
                    GeneroIds = ParseIds(generoIds),
                    StudioIds = ParseIds(studioIds),
                    AutorIds = ParseIds(autorIds),
                    Tipo = tipo,
                    Status = status,
                    NotaMinima = notaMinima,
                    NotaMaxima = notaMaxima,
                    AnoInicio = anoInicio,
                    AnoFim = anoFim,
                    Fonte = fonte,
                    OrdenarPor = ordenarPor,
                    Direcao = direcao,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _showService.GetShowsAsync(filtro);

                var response = new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Shows recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = result.CurrentPage,
                        PageSize = result.PageSize,
                        TotalItems = result.TotalItems,
                        TotalPages = result.TotalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ShowDetalheDto>>> GetShow(int id)
        {
            try
            {
                var show = await _showService.GetShowByIdAsync(id);

                if (show == null)
                {
                    return NotFound(new ApiResponse<ShowDetalheDto>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }

                return Ok(new ApiResponse<ShowDetalheDto>
                {
                    Success = true,
                    Data = show,
                    Message = "Show recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ShowDetalheDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/top-rated
        [HttpGet("top-rated")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowResumoDto>>>> GetTopRated(
            [FromQuery] int limit = 10,
            [FromQuery] string? tipo = null)
        {
            try
            {
                var shows = await _showService.GetTopRatedAsync(limit, tipo);

                return Ok(new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = true,
                    Data = shows,
                    Message = "Top shows recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/shows
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ShowDetalheDto>>> CreateShow([FromBody] ShowCreateUpdateDto showDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<ShowDetalheDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var createdShow = await _showService.CreateShowAsync(showDto);

                return CreatedAtAction(nameof(GetShow), new { id = createdShow.Id },
                    new ApiResponse<ShowDetalheDto>
                    {
                        Success = true,
                        Data = createdShow,
                        Message = "Show criado com sucesso"
                    });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<ShowDetalheDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ShowDetalheDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/shows/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ShowDetalheDto>>> UpdateShow(int id, [FromBody] ShowCreateUpdateDto showDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<ShowDetalheDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var updatedShow = await _showService.UpdateShowAsync(id, showDto);

                return Ok(new ApiResponse<ShowDetalheDto>
                {
                    Success = true,
                    Data = updatedShow,
                    Message = "Show atualizado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<ShowDetalheDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ShowDetalheDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // DELETE: api/shows/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteShow(int id)
        {
            try
            {
                var deleted = await _showService.DeleteShowAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Show deletado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        private static List<int>? ParseIds(string? ids)
        {
            if (string.IsNullOrEmpty(ids))
                return null;

            try
            {
                return ids.Split(',')
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();
            }
            catch
            {
                return null;
            }
        }
    }

    // ================== CONTROLLER DE GÉNEROS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class GenerosController : ControllerBase
    {
        private readonly IGeneroService _generoService;

        public GenerosController(IGeneroService generoService)
        {
            _generoService = generoService;
        }

        // GET: api/generos
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GeneroDto>>>> GetGeneros()
        {
            try
            {
                var generos = await _generoService.GetGenerosAsync();
                return Ok(new ApiResponse<IEnumerable<GeneroDto>>
                {
                    Success = true,
                    Data = generos,
                    Message = "Gêneros recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<GeneroDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/generos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> GetGenero(int id)
        {
            try
            {
                var genero = await _generoService.GetGeneroByIdAsync(id);

                if (genero == null)
                {
                    return NotFound(new ApiResponse<GeneroDto>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                return Ok(new ApiResponse<GeneroDto>
                {
                    Success = true,
                    Data = genero,
                    Message = "Gênero recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GeneroDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/generos/5/shows?page=1&pageSize=10
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ShowResumoDto>>>> GetShowsByGenero(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _generoService.GetShowsByGeneroAsync(id, page, pageSize);
                return Ok(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = true,
                    Data = result,
                    Message = "Shows do gênero recuperados com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/generos
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> CreateGenero([FromBody] GeneroCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<GeneroDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var genero = await _generoService.CreateGeneroAsync(request);

                return CreatedAtAction(nameof(GetGenero), new { id = genero.Id },
                    new ApiResponse<GeneroDto>
                    {
                        Success = true,
                        Data = genero,
                        Message = "Gênero criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GeneroDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/generos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> UpdateGenero(int id, [FromBody] GeneroCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<GeneroDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var genero = await _generoService.UpdateGeneroAsync(id, request);

                return Ok(new ApiResponse<GeneroDto>
                {
                    Success = true,
                    Data = genero,
                    Message = "Gênero atualizado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<GeneroDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GeneroDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // DELETE: api/generos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteGenero(int id)
        {
            try
            {
                var deleted = await _generoService.DeleteGeneroAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Gênero deletado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/generos/5/exists
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> GeneroExists(int id)
        {
            try
            {
                var exists = await _generoService.GeneroExistsAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = exists,
                    Message = exists ? "Gênero existe" : "Gênero não existe"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }

    // ================== CONTROLLER DE STUDIOS ==================
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class StudiosController : ControllerBase
    {
        private readonly IStudioService _studioService;

        public StudiosController(IStudioService studioService)
        {
            _studioService = studioService;
        }

        // GET: api/studios?page=1&pageSize=10&search=nome
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<StudioDto>>>> GetStudios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var studios = await _studioService.GetStudiosAsync(page, pageSize, search);
                return Ok(new ApiResponse<PaginatedResponseDto<StudioDto>>
                {
                    Success = true,
                    Data = studios,
                    Message = "Estúdios recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<StudioDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/studios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudioDto>>> GetStudio(int id)
        {
            try
            {
                var studio = await _studioService.GetStudioByIdAsync(id);

                if (studio == null)
                {
                    return NotFound(new ApiResponse<StudioDto>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                return Ok(new ApiResponse<StudioDto>
                {
                    Success = true,
                    Data = studio,
                    Message = "Estúdio recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<StudioDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/studios/5/shows?page=1&pageSize=10
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ShowResumoDto>>>> GetShowsByStudio(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _studioService.GetShowsByStudioAsync(id, page, pageSize);
                return Ok(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = true,
                    Data = result,
                    Message = "Shows do estúdio recuperados com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/studios
        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudioDto>>> CreateStudio([FromBody] StudioCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<StudioDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var studio = await _studioService.CreateStudioAsync(request);

                return CreatedAtAction(nameof(GetStudio), new { id = studio.Id },
                    new ApiResponse<StudioDto>
                    {
                        Success = true,
                        Data = studio,
                        Message = "Estúdio criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<StudioDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/studios/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StudioDto>>> UpdateStudio(int id, [FromBody] StudioCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<StudioDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var studio = await _studioService.UpdateStudioAsync(id, request);

                return Ok(new ApiResponse<StudioDto>
                {
                    Success = true,
                    Data = studio,
                    Message = "Estúdio atualizado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<StudioDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<StudioDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // DELETE: api/studios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudio(int id)
        {
            try
            {
                var deleted = await _studioService.DeleteStudioAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Estúdio deletado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/studios/5/exists
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> StudioExists(int id)
        {
            try
            {
                var exists = await _studioService.StudioExistsAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = exists,
                    Message = exists ? "Estúdio existe" : "Estúdio não existe"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }

    // ================== CONTROLLER DE AUTORES ==================
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;
        private readonly ApplicationDbContext _context; // Mantido para funcionalidades específicas

        public AutoresController(IAutorService autorService, ApplicationDbContext context)
        {
            _autorService = autorService;
            _context = context; // Para endpoints que não estão no service
        }

        // GET: api/autores?page=1&pageSize=20&search=nome
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<AutorDto>>>> GetAutores(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            try
            {
                var autores = await _autorService.GetAutoresAsync(page, pageSize, search);
                return Ok(new ApiResponse<PaginatedResponseDto<AutorDto>>
                {
                    Success = true,
                    Data = autores,
                    Message = "Autores recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<AutorDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/autores/advanced-search (funcionalidade extra do controller original)
        [HttpGet("advanced-search")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<Autor>>>> GetAutoresAdvanced(
            [FromQuery] string? search = null,
            [FromQuery] Sexualidade? sexualidade = null,
            [FromQuery] int? idadeMin = null,
            [FromQuery] int? idadeMax = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Autores
                    .Include(a => a.ShowsCriados)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(a => a.Nome.Contains(search) ||
                                           (a.Sobre != null && a.Sobre.Contains(search)));
                }

                if (sexualidade.HasValue)
                {
                    query = query.Where(a => a.AutorSexualidade == sexualidade.Value);
                }

                if (idadeMin.HasValue)
                {
                    query = query.Where(a => a.Idade >= idadeMin.Value);
                }

                if (idadeMax.HasValue)
                {
                    query = query.Where(a => a.Idade <= idadeMax.Value);
                }

                // Paginação
                var totalItems = await query.CountAsync();
                var autores = await query
                    .OrderBy(a => a.Nome)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new PaginatedResponseDto<Autor>
                {
                    Data = autores,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                };

                return Ok(new ApiResponse<PaginatedResponseDto<Autor>>
                {
                    Success = true,
                    Data = result,
                    Message = "Autores recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<Autor>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/autores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AutorDto>>> GetAutor(int id)
        {
            try
            {
                var autor = await _autorService.GetAutorByIdAsync(id);

                if (autor == null)
                {
                    return NotFound(new ApiResponse<AutorDto>
                    {
                        Success = false,
                        Message = "Autor não encontrado"
                    });
                }

                return Ok(new ApiResponse<AutorDto>
                {
                    Success = true,
                    Data = autor,
                    Message = "Autor recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AutorDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/autores/5/shows?page=1&pageSize=10
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ShowResumoDto>>>> GetShowsByAutor(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _autorService.GetShowsByAutorAsync(id, page, pageSize);
                return Ok(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = true,
                    Data = result,
                    Message = "Shows do autor recuperados com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaginatedResponseDto<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/autores
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AutorDto>>> CreateAutor([FromBody] AutorCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<AutorDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var autor = await _autorService.CreateAutorAsync(request);

                return CreatedAtAction(nameof(GetAutor), new { id = autor.Id },
                    new ApiResponse<AutorDto>
                    {
                        Success = true,
                        Data = autor,
                        Message = "Autor criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AutorDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/autores/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AutorDto>>> UpdateAutor(int id, [FromBody] AutorCreateUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<AutorDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var autor = await _autorService.UpdateAutorAsync(id, request);

                return Ok(new ApiResponse<AutorDto>
                {
                    Success = true,
                    Data = autor,
                    Message = "Autor atualizado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return NotFound(new ApiResponse<AutorDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AutorDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // DELETE: api/autores/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAutor(int id)
        {
            try
            {
                var deleted = await _autorService.DeleteAutorAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Autor não encontrado"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Autor deletado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/autores/5/exists
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> AutorExists(int id)
        {
            try
            {
                var exists = await _autorService.AutorExistsAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = exists,
                    Message = exists ? "Autor existe" : "Autor não existe"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }

    // ================== CONTROLLER DE PERSONAGENS ==================
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonagensController : ControllerBase
    {
        private readonly IPersonagemService _personagemService;

        public PersonagensController(IPersonagemService personagemService)
        {
            _personagemService = personagemService;
        }

        // GET: api/personagens
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonagemDto>>>> GetPersonagens(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _personagemService.GetPersonagensAsync(page, pageSize, search);

                var response = new ApiResponse<IEnumerable<PersonagemDto>>
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Personagens recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = result.CurrentPage,
                        PageSize = result.PageSize,
                        TotalItems = result.TotalItems,
                        TotalPages = result.TotalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PersonagemDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PersonagemDto>>> GetPersonagem(int id)
        {
            try
            {
                var personagem = await _personagemService.GetPersonagemByIdAsync(id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<PersonagemDto>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                return Ok(new ApiResponse<PersonagemDto>
                {
                    Success = true,
                    Data = personagem,
                    Message = "Personagem recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PersonagemDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/personagens
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersonagemDto>>> CreatePersonagem([FromBody] PersonagemCreateUpdateDto personagemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<PersonagemDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var createdPersonagem = await _personagemService.CreatePersonagemAsync(personagemDto);

                return CreatedAtAction(nameof(GetPersonagem), new { id = createdPersonagem.Id },
                    new ApiResponse<PersonagemDto>
                    {
                        Success = true,
                        Data = createdPersonagem,
                        Message = "Personagem criado com sucesso"
                    });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<PersonagemDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PersonagemDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/personagens/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PersonagemDto>>> UpdatePersonagem(int id, [FromBody] PersonagemCreateUpdateDto personagemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<PersonagemDto>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Verificar se o personagem existe
                var exists = await _personagemService.PersonagemExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new ApiResponse<PersonagemDto>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                var updatedPersonagem = await _personagemService.UpdatePersonagemAsync(id, personagemDto);

                return Ok(new ApiResponse<PersonagemDto>
                {
                    Success = true,
                    Data = updatedPersonagem,
                    Message = "Personagem atualizado com sucesso"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<PersonagemDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PersonagemDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // DELETE: api/personagens/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePersonagem(int id)
        {
            try
            {
                var deleted = await _personagemService.DeletePersonagemAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Personagem deletado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // Métodos adicionais específicos que você pode querer manter

        // GET: api/personagens/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowResumoDto>>>> GetShowsByPersonagem(int id)
        {
            try
            {
                var personagem = await _personagemService.GetPersonagemByIdAsync(id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<IEnumerable<ShowResumoDto>>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                return Ok(new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = true,
                    Data = personagem.Shows,
                    Message = "Shows do personagem recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowResumoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/check-exists/5
        [HttpGet("check-exists/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPersonagemExists(int id)
        {
            try
            {
                var exists = await _personagemService.PersonagemExistsAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = exists,
                    Message = exists ? "Personagem existe" : "Personagem não existe"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }

    // ================== CONTROLLER DE ESTATISTICAS ==================
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class EstatisticasController : ControllerBase
    {
        private readonly IEstatisticasService _estatisticasService;

        public EstatisticasController(IEstatisticasService estatisticasService)
        {
            _estatisticasService = estatisticasService;
        }

        // GET: api/estatisticas/gerais
        [HttpGet("gerais")]
        public async Task<ActionResult<ApiResponse<EstatisticasDto>>> GetEstatisticasGerais()
        {
            try
            {
                var estatisticas = await _estatisticasService.GetEstatisticasGeraisAsync();

                return Ok(new ApiResponse<EstatisticasDto>
                {
                    Success = true,
                    Data = estatisticas,
                    Message = "Estatísticas gerais recuperadas com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<EstatisticasDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/shows
        [HttpGet("shows")]
        public async Task<ActionResult<ApiResponse<RelatorioShowsDto>>> GetRelatorioShows(
            [FromQuery] DateTime? inicio = null,
            [FromQuery] DateTime? fim = null)
        {
            try
            {
                // Validação das datas
                if (inicio.HasValue && fim.HasValue && inicio.Value > fim.Value)
                {
                    return BadRequest(new ApiResponse<RelatorioShowsDto>
                    {
                        Success = false,
                        Message = "A data de início deve ser anterior à data de fim"
                    });
                }

                var relatorio = await _estatisticasService.GetRelatorioShowsAsync(inicio, fim);

                return Ok(new ApiResponse<RelatorioShowsDto>
                {
                    Success = true,
                    Data = relatorio,
                    Message = "Relatório de shows recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<RelatorioShowsDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/resumo
        [HttpGet("resumo")]
        public async Task<ActionResult<ApiResponse<EstatisticasResumoDto>>> GetEstatisticasResumo()
        {
            try
            {
                var estatisticas = await _estatisticasService.GetEstatisticasGeraisAsync();

                // Criar um resumo com apenas as informações mais importantes
                var resumo = new EstatisticasResumoDto
                {
                    TotalShows = estatisticas.TotalShows,
                    TotalAnimes = estatisticas.TotalAnimes,
                    TotalMangas = estatisticas.TotalMangas,
                    TotalUtilizadores = estatisticas.TotalUtilizadores,
                    TotalPersonagens = estatisticas.TotalPersonagens,
                    NotaMediaGeral = (double) estatisticas.NotaMediaGeral,
                    GeneroMaisPopular = estatisticas.GenerosPopulares?.FirstOrDefault()?.Nome ?? "N/A",
                    ShowMaisPopular = estatisticas.ShowsPopulares?.FirstOrDefault()?.Nome ?? "N/A"
                };

                return Ok(new ApiResponse<EstatisticasResumoDto>
                {
                    Success = true,
                    Data = resumo,
                    Message = "Resumo de estatísticas recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<EstatisticasResumoDto>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/generos-populares
        [HttpGet("generos-populares")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GeneroPopularDto>>>> GetGenerosPopulares(
            [FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return BadRequest(new ApiResponse<IEnumerable<GeneroPopularDto>>
                    {
                        Success = false,
                        Message = "O limite deve estar entre 1 e 50"
                    });
                }

                var estatisticas = await _estatisticasService.GetEstatisticasGeraisAsync();
                var generosPopulares = estatisticas.GenerosPopulares?.Take(limit) ?? new List<GeneroPopularDto>();

                return Ok(new ApiResponse<IEnumerable<GeneroPopularDto>>
                {
                    Success = true,
                    Data = generosPopulares,
                    Message = "Gêneros populares recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<GeneroPopularDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/shows-populares
        [HttpGet("shows-populares")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowPopularDto>>>> GetShowsPopulares(
            [FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return BadRequest(new ApiResponse<IEnumerable<ShowPopularDto>>
                    {
                        Success = false,
                        Message = "O limite deve estar entre 1 e 50"
                    });
                }

                var estatisticas = await _estatisticasService.GetEstatisticasGeraisAsync();
                var showsPopulares = estatisticas.ShowsPopulares?.Take(limit) ?? new List<ShowPopularDto>();

                return Ok(new ApiResponse<IEnumerable<ShowPopularDto>>
                {
                    Success = true,
                    Data = showsPopulares,
                    Message = "Shows populares recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowPopularDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/studios-populares
        [HttpGet("studios-populares")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudioPopularDto>>>> GetStudiosPopulares(
            [FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return BadRequest(new ApiResponse<IEnumerable<StudioPopularDto>>
                    {
                        Success = false,
                        Message = "O limite deve estar entre 1 e 50"
                    });
                }

                var estatisticas = await _estatisticasService.GetEstatisticasGeraisAsync();
                var studiosPopulares = estatisticas.StudiosPopulares?.Take(limit) ?? new List<StudioPopularDto>();

                return Ok(new ApiResponse<IEnumerable<StudioPopularDto>>
                {
                    Success = true,
                    Data = studiosPopulares,
                    Message = "Studios populares recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<StudioPopularDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/shows/por-ano
        [HttpGet("shows/por-ano")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowsPorAnoDto>>>> GetShowsPorAno(
            [FromQuery] DateTime? inicio = null,
            [FromQuery] DateTime? fim = null)
        {
            try
            {
                var relatorio = await _estatisticasService.GetRelatorioShowsAsync(inicio, fim);

                return Ok(new ApiResponse<IEnumerable<ShowsPorAnoDto>>
                {
                    Success = true,
                    Data = relatorio.ShowsPorAno ?? new List<ShowsPorAnoDto>(),
                    Message = "Estatísticas de shows por ano recuperadas com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowsPorAnoDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/shows/por-genero
        [HttpGet("shows/por-genero")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowsPorGeneroDto>>>> GetShowsPorGenero(
            [FromQuery] DateTime? inicio = null,
            [FromQuery] DateTime? fim = null)
        {
            try
            {
                var relatorio = await _estatisticasService.GetRelatorioShowsAsync(inicio, fim);

                return Ok(new ApiResponse<IEnumerable<ShowsPorGeneroDto>>
                {
                    Success = true,
                    Data = relatorio.ShowsPorGenero ?? new List<ShowsPorGeneroDto>(),
                    Message = "Estatísticas de shows por gênero recuperadas com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowsPorGeneroDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/estatisticas/shows/por-status
        [HttpGet("shows/por-status")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShowsPorStatusDto>>>> GetShowsPorStatus(
            [FromQuery] DateTime? inicio = null,
            [FromQuery] DateTime? fim = null)
        {
            try
            {
                var relatorio = await _estatisticasService.GetRelatorioShowsAsync(inicio, fim);

                return Ok(new ApiResponse<IEnumerable<ShowsPorStatusDto>>
                {
                    Success = true,
                    Data = relatorio.ShowsPorStatus ?? new List<ShowsPorStatusDto>(),
                    Message = "Estatísticas de shows por status recuperadas com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<ShowsPorStatusDto>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }
}