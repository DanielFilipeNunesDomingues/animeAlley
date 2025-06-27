using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Controllers.Api
{

    // ================== CONTROLLER PRINCIPAL DE SHOWS (ANIMES/MANGAS) ==================
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/shows
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetShows(
            [FromQuery] string? search = null,
            [FromQuery] int? generoId = null,
            [FromQuery] int? studioId = null,
            [FromQuery] Tipo? tipo = null,
            [FromQuery] Status? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.Personagens)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(s => s.Nome.Contains(search) || s.Sinopse.Contains(search));
                }

                if (generoId.HasValue)
                {
                    query = query.Where(s => s.GenerosShows.Any(g => g.Id == generoId.Value));
                }

                if (studioId.HasValue)
                {
                    query = query.Where(s => s.StudioFK == studioId.Value);
                }

                if (tipo.HasValue)
                {
                    query = query.Where(s => s.Tipo == tipo.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(s => s.Status == status.Value);
                }

                // Paginação
                var totalItems = await query.CountAsync();
                var shows = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = shows,
                    Message = "Shows recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Show>>> GetShow(int id)
        {
            try
            {
                var show = await _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.Personagens)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (show == null)
                {
                    return NotFound(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }

                // Incrementar views
                show.Views++;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Show>
                {
                    Success = true,
                    Data = show,
                    Message = "Show recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Show>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/animes
        [HttpGet("animes")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetAnimes(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Where(s => s.Tipo == Tipo.Anime)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(s => s.Nome.Contains(search) || s.Sinopse.Contains(search));
                }

                var totalItems = await query.CountAsync();
                var animes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = animes,
                    Message = "Animes recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/mangas
        [HttpGet("mangas")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetMangas(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Where(s => s.Tipo == Tipo.Manga)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(s => s.Nome.Contains(search) || s.Sinopse.Contains(search));
                }

                var totalItems = await query.CountAsync();
                var mangas = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = mangas,
                    Message = "Mangas recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/top-rated
        [HttpGet("top-rated")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetTopRated(
            [FromQuery] int limit = 10,
            [FromQuery] Tipo? tipo = null)
        {
            try
            {
                var query = _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .AsQueryable();

                if (tipo.HasValue)
                {
                    query = query.Where(s => s.Tipo == tipo);
                }

                var topShows = await query
                    .OrderByDescending(s => s.Nota)
                    .Take(limit)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = topShows,
                    Message = "Top shows recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/shows/trending
        [HttpGet("trending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetTrending(
            [FromQuery] int limit = 10,
            [FromQuery] Tipo? tipo = null)
        {
            try
            {
                var query = _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .AsQueryable();

                if (tipo.HasValue)
                {
                    query = query.Where(s => s.Tipo == tipo);
                }

                var trendingShows = await query
                    .OrderByDescending(s => s.Views)
                    .Take(limit)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = trendingShows,
                    Message = "Shows em alta recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/shows
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Show>>> CreateShow([FromBody] CreateShowRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Validações adicionais
                if (request.StudioFK <= 0)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "StudioFK deve ser maior que zero"
                    });
                }

                if (request.AutorFK <= 0)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "AutorFK deve ser maior que zero"
                    });
                }

                // Validar se Studio e Autor existem
                var studioExists = await _context.Studios.AnyAsync(s => s.Id == request.StudioFK);
                if (!studioExists)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Studio não encontrado"
                    });
                }

                var autorExists = await _context.Autores.AnyAsync(a => a.Id == request.AutorFK);
                if (!autorExists)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Autor não encontrado"
                    });
                }

                // Converter NotaAux para Nota se necessário
                decimal nota = 0;
                if (!string.IsNullOrEmpty(request.NotaAux))
                {
                    if (!decimal.TryParse(request.NotaAux.Replace(',', '.'),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out nota))
                    {
                        return BadRequest(new ApiResponse<Show>
                        {
                            Success = false,
                            Message = "Formato de nota inválido"
                        });
                    }
                }
                else if (request.Nota.HasValue)
                {
                    nota = request.Nota.Value;
                }

                var show = new Show
                {
                    Nome = request.Nome,
                    Sinopse = request.Sinopse,
                    Tipo = request.Tipo,
                    Status = request.Status,
                    Nota = nota,
                    Ano = request.Ano,
                    Imagem = request.Imagem ?? string.Empty,
                    Banner = request.Banner ?? string.Empty,
                    Trailer = request.Trailer ?? string.Empty,
                    Fonte = request.Fonte,
                    StudioFK = request.StudioFK,
                    AutorFK = request.AutorFK,
                    Views = 0
                };

                _context.Shows.Add(show);
                await _context.SaveChangesAsync();

                // Associar gêneros se fornecidos
                if (request.GeneroIds != null && request.GeneroIds.Any())
                {
                    var generos = await _context.Generos
                        .Where(g => request.GeneroIds.Contains(g.Id))
                        .ToListAsync();

                    if (generos.Count != request.GeneroIds.Count())
                    {
                        // Alguns gêneros não foram encontrados
                        var foundIds = generos.Select(g => g.Id);
                        var notFoundIds = request.GeneroIds.Except(foundIds);

                        return BadRequest(new ApiResponse<Show>
                        {
                            Success = false,
                            Message = $"Gêneros não encontrados: {string.Join(", ", notFoundIds)}"
                        });
                    }

                    show.GenerosShows = generos;
                    await _context.SaveChangesAsync();
                }

                // Recarregar o show com todas as relações para retornar
                var createdShow = await _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.Personagens)
                    .FirstOrDefaultAsync(s => s.Id == show.Id);

                return CreatedAtAction(nameof(GetShow), new { id = show.Id },
                    new ApiResponse<Show>
                    {
                        Success = true,
                        Data = createdShow,
                        Message = "Show criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Show>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/shows/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Show>>> UpdateShow(int id, [FromBody] UpdateShowRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "ID do show não coincide"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var show = await _context.Shows
                    .Include(s => s.GenerosShows)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (show == null)
                {
                    return NotFound(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }

                // Validações adicionais
                if (request.StudioFK <= 0)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "StudioFK deve ser maior que zero"
                    });
                }

                if (request.AutorFK <= 0)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "AutorFK deve ser maior que zero"
                    });
                }

                // Validar se Studio e Autor existem
                var studioExists = await _context.Studios.AnyAsync(s => s.Id == request.StudioFK);
                if (!studioExists)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Studio não encontrado"
                    });
                }

                var autorExists = await _context.Autores.AnyAsync(a => a.Id == request.AutorFK);
                if (!autorExists)
                {
                    return BadRequest(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Autor não encontrado"
                    });
                }

                // Converter NotaAux para Nota se necessário
                decimal nota = show.Nota; // Manter valor atual se não for fornecido novo
                if (!string.IsNullOrEmpty(request.NotaAux))
                {
                    if (!decimal.TryParse(request.NotaAux.Replace(',', '.'),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out nota))
                    {
                        return BadRequest(new ApiResponse<Show>
                        {
                            Success = false,
                            Message = "Formato de nota inválido"
                        });
                    }
                }
                else if (request.Nota.HasValue)
                {
                    nota = request.Nota.Value;
                }

                // Atualizar propriedades
                show.Nome = request.Nome;
                show.Sinopse = request.Sinopse;
                show.Tipo = request.Tipo;
                show.Status = request.Status;
                show.Nota = nota;
                show.Ano = request.Ano;

                // Só atualizar se valores forem fornecidos
                if (!string.IsNullOrEmpty(request.Imagem))
                    show.Imagem = request.Imagem;
                if (!string.IsNullOrEmpty(request.Banner))
                    show.Banner = request.Banner;
                if (request.Trailer != null)
                    show.Trailer = request.Trailer;

                show.Fonte = request.Fonte;
                show.StudioFK = request.StudioFK;
                show.AutorFK = request.AutorFK;

                // Atualizar gêneros se fornecidos
                if (request.GeneroIds != null)
                {
                    // Limpar gêneros existentes
                    show.GenerosShows.Clear();

                    if (request.GeneroIds.Any())
                    {
                        var generos = await _context.Generos
                            .Where(g => request.GeneroIds.Contains(g.Id))
                            .ToListAsync();

                        if (generos.Count != request.GeneroIds.Count())
                        {
                            var foundIds = generos.Select(g => g.Id);
                            var notFoundIds = request.GeneroIds.Except(foundIds);

                            return BadRequest(new ApiResponse<Show>
                            {
                                Success = false,
                                Message = $"Gêneros não encontrados: {string.Join(", ", notFoundIds)}"
                            });
                        }

                        show.GenerosShows = generos;
                    }
                }

                await _context.SaveChangesAsync();

                // Recarregar o show com todas as relações para retornar
                var updatedShow = await _context.Shows
                    .Include(s => s.GenerosShows)
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.Personagens)
                    .FirstOrDefaultAsync(s => s.Id == id);

                return Ok(new ApiResponse<Show>
                {
                    Success = true,
                    Data = updatedShow,
                    Message = "Show atualizado com sucesso"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowExists(id))
                {
                    return NotFound(new ApiResponse<Show>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Show>
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
                var show = await _context.Shows
                    .Include(s => s.GenerosShows)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (show == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Show não encontrado"
                    });
                }

                // Limpar as relações many-to-many antes de deletar
                show.GenerosShows.Clear();

                _context.Shows.Remove(show);
                await _context.SaveChangesAsync();

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

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }

    // ================== CONTROLLER DE GÉNEROS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenerosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/generos
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Genero>>>> GetGeneros()
        {
            try
            {
                var generos = await _context.Generos.ToListAsync();
                return Ok(new ApiResponse<IEnumerable<Genero>>
                {
                    Success = true,
                    Data = generos,
                    Message = "Gêneros recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Genero>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/generos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Genero>>> GetGenero(int id)
        {
            try
            {
                var genero = await _context.Generos.FindAsync(id);

                if (genero == null)
                {
                    return NotFound(new ApiResponse<Genero>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                return Ok(new ApiResponse<Genero>
                {
                    Success = true,
                    Data = genero,
                    Message = "Gênero recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Genero>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/generos/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetShowsByGenero(int id)
        {
            try
            {
                var genero = await _context.Generos
                    .Include(g => g.Shows)
                        .ThenInclude(s => s.Studio)
                    .Include(g => g.Shows)
                        .ThenInclude(s => s.Autor)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (genero == null)
                {
                    return NotFound(new ApiResponse<IEnumerable<Show>>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = genero.Shows,
                    Message = "Shows do gênero recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/generos
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Genero>>> CreateGenero([FromBody] CreateGeneroRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Genero>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var genero = new Genero
                {
                    GeneroNome = request.GeneroNome
                };

                _context.Generos.Add(genero);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGenero), new { id = genero.Id },
                    new ApiResponse<Genero>
                    {
                        Success = true,
                        Data = genero,
                        Message = "Gênero criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Genero>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/generos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Genero>>> UpdateGenero(int id, [FromBody] UpdateGeneroRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ApiResponse<Genero>
                    {
                        Success = false,
                        Message = "ID do gênero não coincide"
                    });
                }

                var genero = await _context.Generos.FindAsync(id);
                if (genero == null)
                {
                    return NotFound(new ApiResponse<Genero>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                genero.GeneroNome = request.GeneroNome;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Genero>
                {
                    Success = true,
                    Data = genero,
                    Message = "Gênero atualizado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Genero>
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
                var genero = await _context.Generos.FindAsync(id);
                if (genero == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Gênero não encontrado"
                    });
                }

                _context.Generos.Remove(genero);
                await _context.SaveChangesAsync();

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
    }

    // ================== CONTROLLER DE STUDIOS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class StudiosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudiosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/studios
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Studio>>>> GetStudios()
        {
            try
            {
                var studios = await _context.Studios.ToListAsync();
                return Ok(new ApiResponse<IEnumerable<Studio>>
                {
                    Success = true,
                    Data = studios,
                    Message = "Estúdios recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Studio>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/studios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Studio>>> GetStudio(int id)
        {
            try
            {
                var studio = await _context.Studios.FindAsync(id);

                if (studio == null)
                {
                    return NotFound(new ApiResponse<Studio>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                return Ok(new ApiResponse<Studio>
                {
                    Success = true,
                    Data = studio,
                    Message = "Estúdio recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Studio>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/studios/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetShowsByStudio(int id)
        {
            try
            {
                var studio = await _context.Studios
                    .Include(s => s.ShowsDesenvolvidos)
                        .ThenInclude(sh => sh.GenerosShows)
                    .Include(s => s.ShowsDesenvolvidos)
                        .ThenInclude(sh => sh.Autor)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (studio == null)
                {
                    return NotFound(new ApiResponse<IEnumerable<Show>>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = studio.ShowsDesenvolvidos,
                    Message = "Shows do estúdio recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/studios
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Studio>>> CreateStudio([FromBody] CreateStudioRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Studio>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var studio = new Studio
                {
                    Nome = request.Nome,
                    Foto = request.Foto,
                    Sobre = request.Sobre,
                    Fundado = request.Fundado,
                    Fechado = request.Fechado,
                    Status = request.Status
                };

                _context.Studios.Add(studio);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStudio), new { id = studio.Id },
                    new ApiResponse<Studio>
                    {
                        Success = true,
                        Data = studio,
                        Message = "Estúdio criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Studio>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/studios/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Studio>>> UpdateStudio(int id, [FromBody] UpdateStudioRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ApiResponse<Studio>
                    {
                        Success = false,
                        Message = "ID do estúdio não coincide"
                    });
                }

                var studio = await _context.Studios.FindAsync(id);
                if (studio == null)
                {
                    return NotFound(new ApiResponse<Studio>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                studio.Nome = request.Nome;
                studio.Foto = request.Foto;
                studio.Sobre = request.Sobre;
                studio.Fundado = request.Fundado;
                studio.Fechado = request.Fechado;
                studio.Status = request.Status;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Studio>
                {
                    Success = true,
                    Data = studio,
                    Message = "Estúdio atualizado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Studio>
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
                var studio = await _context.Studios.FindAsync(id);
                if (studio == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Estúdio não encontrado"
                    });
                }

                _context.Studios.Remove(studio);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Estúdio deletado com sucesso"
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
    }

    // ================== CONTROLLER DE UTILIZADORES ==================]
    [ApiController]
    [Route("api/[controller]")]
    public class UtilizadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/utilizadores
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Utilizador>>>> GetUtilizadores(
            [FromQuery] string? search = null,
            [FromQuery] bool? isAdmin = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Utilizadores
                    .Include(u => u.Lista)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => u.Nome.Contains(search) || u.UserName.Contains(search));
                }

                if (isAdmin.HasValue)
                {
                    query = query.Where(u => u.isAdmin == isAdmin.Value);
                }

                // Paginação
                var totalItems = await query.CountAsync();
                var utilizadores = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Utilizador>>
                {
                    Success = true,
                    Data = utilizadores,
                    Message = "Utilizadores recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Utilizador>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/utilizadores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Utilizador>>> GetUtilizador(int id)
        {
            try
            {
                var utilizador = await _context.Utilizadores
                    .Include(u => u.Lista)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (utilizador == null)
                {
                    return NotFound(new ApiResponse<Utilizador>
                    {
                        Success = false,
                        Message = "Utilizador não encontrado"
                    });
                }

                return Ok(new ApiResponse<Utilizador>
                {
                    Success = true,
                    Data = utilizador,
                    Message = "Utilizador recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Utilizador>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }

    // ================== CONTROLLER DE LISTAS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class ListasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/listas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lista>>> GetListas(
            [FromQuery] int? utilizadorId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Listas
                .Include(l => l.Utilizador)
                .Include(l => l.ListaShows)
                    .ThenInclude(ls => ls.Show)
                .AsQueryable();

            // Filtros
            if (utilizadorId.HasValue)
            {
                query = query.Where(l => l.UtilizadorId == utilizadorId.Value);
            }

            // Paginação
            var totalItems = await query.CountAsync();
            var listas = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = listas,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/listas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lista>> GetLista(int id)
        {
            var lista = await _context.Listas
                .Include(l => l.Utilizador)
                .Include(l => l.ListaShows)
                    .ThenInclude(ls => ls.Show)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lista == null)
            {
                return NotFound();
            }

            return lista;
        }

        // GET: api/listas/utilizador/5
        [HttpGet("utilizador/{utilizadorId}")]
        public async Task<ActionResult<Lista>> GetListaByUtilizador(int utilizadorId)
        {
            var lista = await _context.Listas
                .Include(l => l.Utilizador)
                .Include(l => l.ListaShows)
                    .ThenInclude(ls => ls.Show)
                .FirstOrDefaultAsync(l => l.UtilizadorId == utilizadorId);

            if (lista == null)
            {
                return NotFound();
            }

            return lista;
        }

        // POST: api/listas
        [HttpPost]
        public async Task<ActionResult<Lista>> CreateLista(Lista lista)
        {
            _context.Listas.Add(lista);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLista), new { id = lista.Id }, lista);
        }

        // PUT: api/listas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLista(int id, Lista lista)
        {
            if (id != lista.Id)
            {
                return BadRequest();
            }

            _context.Entry(lista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/listas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLista(int id)
        {
            var lista = await _context.Listas.FindAsync(id);
            if (lista == null)
            {
                return NotFound();
            }

            _context.Listas.Remove(lista);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaExists(int id)
        {
            return _context.Listas.Any(e => e.Id == id);
        }
    }

    // ================== CONTROLLER DE LISTA SHOWS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class ListaShowsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListaShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/listashows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaShows>>> GetListaShows(
            [FromQuery] int? listaId = null,
            [FromQuery] int? showId = null,
            [FromQuery] ListaStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.ListaShows
                .Include(ls => ls.Lista)
                .Include(ls => ls.Show)
                .AsQueryable();

            // Filtros
            if (listaId.HasValue)
            {
                query = query.Where(ls => ls.ListaId == listaId.Value);
            }

            if (showId.HasValue)
            {
                query = query.Where(ls => ls.ShowId == showId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(ls => ls.ListaStatus == status.Value);
            }

            // Paginação
            var totalItems = await query.CountAsync();
            var listaShows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = listaShows,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/listashows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaShows>> GetListaShow(int id)
        {
            var listaShow = await _context.ListaShows
                .Include(ls => ls.Lista)
                .Include(ls => ls.Show)
                .FirstOrDefaultAsync(ls => ls.Id == id);

            if (listaShow == null)
            {
                return NotFound();
            }

            return listaShow;
        }

        // GET: api/listashows/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ListaShows>>> GetListaShowsByStatus(
            ListaStatus status,
            [FromQuery] int? listaId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.ListaShows
                .Include(ls => ls.Lista)
                .Include(ls => ls.Show)
                .Where(ls => ls.ListaStatus == status)
                .AsQueryable();

            if (listaId.HasValue)
            {
                query = query.Where(ls => ls.ListaId == listaId.Value);
            }

            var totalItems = await query.CountAsync();
            var listaShows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = listaShows,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // POST: api/listashows
        [HttpPost]
        public async Task<ActionResult<ListaShows>> CreateListaShow(ListaShows listaShow)
        {
            // Verificar se o show já existe na lista
            var existingItem = await _context.ListaShows
                .FirstOrDefaultAsync(ls => ls.ListaId == listaShow.ListaId && ls.ShowId == listaShow.ShowId);

            if (existingItem != null)
            {
                return Conflict("Este show já existe na lista.");
            }

            _context.ListaShows.Add(listaShow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetListaShow), new { id = listaShow.Id }, listaShow);
        }

        // PUT: api/listashows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListaShow(int id, ListaShows listaShow)
        {
            if (id != listaShow.Id)
            {
                return BadRequest();
            }

            _context.Entry(listaShow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaShowExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PUT: api/listashows/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateListaShowStatus(int id, [FromBody] ListaStatus status)
        {
            var listaShow = await _context.ListaShows.FindAsync(id);
            if (listaShow == null)
            {
                return NotFound();
            }

            listaShow.ListaStatus = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/listashows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaShow(int id)
        {
            var listaShow = await _context.ListaShows.FindAsync(id);
            if (listaShow == null)
            {
                return NotFound();
            }

            _context.ListaShows.Remove(listaShow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaShowExists(int id)
        {
            return _context.ListaShows.Any(e => e.Id == id);
        }
    }

    // ================== CONTROLLER DE AUTORES ==================
    [ApiController]
    [Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AutoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/autores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Autor>>> GetAutores(
            [FromQuery] string? search = null,
            [FromQuery] Sexualidade? sexualidade = null,
            [FromQuery] int? idadeMin = null,
            [FromQuery] int? idadeMax = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Autores
                .Include(a => a.ShowsCriados)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Nome.Contains(search) || a.Sobre.Contains(search));
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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = autores,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/autores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> GetAutor(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        // GET: api/autores/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<IEnumerable<Show>>> GetShowsByAutor(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return Ok(autor.ShowsCriados);
        }

        // GET: api/autores/populares
        [HttpGet("populares")]
        public async Task<ActionResult<IEnumerable<Autor>>> GetAutoresPopulares(
            [FromQuery] int limit = 10)
        {
            var autoresPopulares = await _context.Autores
                .Include(a => a.ShowsCriados)
                .OrderByDescending(a => a.ShowsCriados.Count)
                .Take(limit)
                .ToListAsync();

            return Ok(autoresPopulares);
        }

        // POST: api/autores
        [HttpPost]
        public async Task<ActionResult<Autor>> CreateAutor(Autor autor)
        {
            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, autor);
        }

        // PUT: api/autores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAutor(int id, Autor autor)
        {
            if (id != autor.Id)
            {
                return BadRequest();
            }

            _context.Entry(autor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutorExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/autores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AutorExists(int id)
        {
            return _context.Autores.Any(e => e.Id == id);
        }
    }

    // ================== CONTROLLER DE PERSONAGENS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class PersonagensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/personagens
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Personagem>>>> GetPersonagens(
            [FromQuery] string? search = null,
            [FromQuery] TiposPersonagem? tipoPersonagem = null,
            [FromQuery] Sexualidade? sexualidade = null,
            [FromQuery] int? idadeMin = null,
            [FromQuery] int? idadeMax = null,
            [FromQuery] int? showId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Personagens
                    .Include(p => p.Shows)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Nome.Contains(search) || p.Sinopse.Contains(search));
                }

                if (tipoPersonagem.HasValue)
                {
                    query = query.Where(p => p.TipoPersonagem == tipoPersonagem.Value);
                }

                if (sexualidade.HasValue)
                {
                    query = query.Where(p => p.PersonagemSexualidade == sexualidade.Value);
                }

                if (idadeMin.HasValue)
                {
                    query = query.Where(p => p.Idade >= idadeMin.Value);
                }

                if (idadeMax.HasValue)
                {
                    query = query.Where(p => p.Idade <= idadeMax.Value);
                }

                if (showId.HasValue)
                {
                    query = query.Where(p => p.Shows.Any(s => s.Id == showId.Value));
                }

                // Paginação
                var totalItems = await query.CountAsync();
                var personagens = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = true,
                    Data = personagens,
                    Message = "Personagens recuperados com sucesso",
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Personagem>>> GetPersonagem(int id)
        {
            try
            {
                var personagem = await _context.Personagens
                    .Include(p => p.Shows)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                return Ok(new ApiResponse<Personagem>
                {
                    Success = true,
                    Data = personagem,
                    Message = "Personagem recuperado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Personagem>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Show>>>> GetShowsByPersonagem(int id)
        {
            try
            {
                var personagem = await _context.Personagens
                    .Include(p => p.Shows)
                        .ThenInclude(s => s.GenerosShows)
                    .Include(p => p.Shows)
                        .ThenInclude(s => s.Studio)
                    .Include(p => p.Shows)
                        .ThenInclude(s => s.Autor)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<IEnumerable<Show>>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                return Ok(new ApiResponse<IEnumerable<Show>>
                {
                    Success = true,
                    Data = personagem.Shows,
                    Message = "Shows do personagem recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Show>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/protagonistas
        [HttpGet("protagonistas")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Personagem>>>> GetProtagonistas(
            [FromQuery] int limit = 10,
            [FromQuery] int? showId = null)
        {
            try
            {
                var query = _context.Personagens
                    .Include(p => p.Shows)
                    .Where(p => p.TipoPersonagem == TiposPersonagem.Protagonista)
                    .AsQueryable();

                if (showId.HasValue)
                {
                    query = query.Where(p => p.Shows.Any(s => s.Id == showId.Value));
                }

                var protagonistas = await query
                    .Take(limit)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = true,
                    Data = protagonistas,
                    Message = "Protagonistas recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // GET: api/personagens/populares
        [HttpGet("populares")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Personagem>>>> GetPersonagensPopulares(
            [FromQuery] int limit = 10)
        {
            try
            {
                var personagensPopulares = await _context.Personagens
                    .Include(p => p.Shows)
                    .OrderByDescending(p => p.Shows.Count)
                    .Take(limit)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = true,
                    Data = personagensPopulares,
                    Message = "Personagens populares recuperados com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<Personagem>>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // POST: api/personagens
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Personagem>>> CreatePersonagem([FromBody] CreatePersonagemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var personagem = new Personagem
                {
                    Nome = request.Nome,
                    TipoPersonagem = request.TipoPersonagem,
                    PersonagemSexualidade = request.PersonagemSexualidade,
                    Idade = request.Idade,
                    DataNasc = request.DataNasc,
                    Sinopse = request.Sinopse ?? string.Empty,
                    Foto = request.Foto ?? string.Empty
                };

                _context.Personagens.Add(personagem);
                await _context.SaveChangesAsync();

                // Associar shows se fornecidos
                if (request.ShowIds != null && request.ShowIds.Any())
                {
                    var shows = await _context.Shows
                        .Where(s => request.ShowIds.Contains(s.Id))
                        .ToListAsync();

                    if (shows.Count != request.ShowIds.Count())
                    {
                        var foundIds = shows.Select(s => s.Id);
                        var notFoundIds = request.ShowIds.Except(foundIds);

                        return BadRequest(new ApiResponse<Personagem>
                        {
                            Success = false,
                            Message = $"Shows não encontrados: {string.Join(", ", notFoundIds)}"
                        });
                    }

                    personagem.Shows = shows;
                    await _context.SaveChangesAsync();
                }

                // Recarregar o personagem com todas as relações para retornar
                var createdPersonagem = await _context.Personagens
                    .Include(p => p.Shows)
                    .FirstOrDefaultAsync(p => p.Id == personagem.Id);

                return CreatedAtAction(nameof(GetPersonagem), new { id = personagem.Id },
                    new ApiResponse<Personagem>
                    {
                        Success = true,
                        Data = createdPersonagem,
                        Message = "Personagem criado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Personagem>
                {
                    Success = false,
                    Message = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        // PUT: api/personagens/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Personagem>>> UpdatePersonagem(int id, [FromBody] UpdatePersonagemRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "ID do personagem não coincide"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var personagem = await _context.Personagens
                    .Include(p => p.Shows)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                // Atualizar propriedades
                personagem.Nome = request.Nome;
                personagem.TipoPersonagem = request.TipoPersonagem;
                personagem.PersonagemSexualidade = request.PersonagemSexualidade;
                personagem.Idade = request.Idade;
                personagem.DataNasc = request.DataNasc;
                personagem.Sinopse = request.Sinopse ?? string.Empty;

                // Só atualizar a foto se for fornecida
                if (!string.IsNullOrEmpty(request.Foto))
                    personagem.Foto = request.Foto;

                // Atualizar shows se fornecidos
                if (request.ShowIds != null)
                {
                    // Limpar shows existentes
                    personagem.Shows.Clear();

                    if (request.ShowIds.Any())
                    {
                        var shows = await _context.Shows
                            .Where(s => request.ShowIds.Contains(s.Id))
                            .ToListAsync();

                        if (shows.Count != request.ShowIds.Count())
                        {
                            var foundIds = shows.Select(s => s.Id);
                            var notFoundIds = request.ShowIds.Except(foundIds);

                            return BadRequest(new ApiResponse<Personagem>
                            {
                                Success = false,
                                Message = $"Shows não encontrados: {string.Join(", ", notFoundIds)}"
                            });
                        }

                        personagem.Shows = shows;
                    }
                }

                await _context.SaveChangesAsync();

                // Recarregar o personagem com todas as relações para retornar
                var updatedPersonagem = await _context.Personagens
                    .Include(p => p.Shows)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return Ok(new ApiResponse<Personagem>
                {
                    Success = true,
                    Data = updatedPersonagem,
                    Message = "Personagem atualizado com sucesso"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonagemExists(id))
                {
                    return NotFound(new ApiResponse<Personagem>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Personagem>
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
                var personagem = await _context.Personagens
                    .Include(p => p.Shows)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (personagem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Personagem não encontrado"
                    });
                }

                // Limpar as relações many-to-many antes de deletar
                personagem.Shows.Clear();

                _context.Personagens.Remove(personagem);
                await _context.SaveChangesAsync();

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

        private bool PersonagemExists(int id)
        {
            return _context.Personagens.Any(e => e.Id == id);
        }
    }
}