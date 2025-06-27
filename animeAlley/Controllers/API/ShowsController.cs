using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;

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
        public async Task<ActionResult<IEnumerable<Show>>> GetShows(
            [FromQuery] string? search = null,
            [FromQuery] int? generoId = null,
            [FromQuery] int? studioId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
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
                query = query.Where(s => s.GenerosShows.Any(st => st.Id == studioId.Value));
            }

            // Paginação
            var totalItems = await query.CountAsync();
            var shows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = shows,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/shows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Show>> GetShow(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Personagens)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            return show;
        }

        // GET: api/shows/animes
        [HttpGet("animes")]
        public async Task<ActionResult<IEnumerable<Show>>> GetAnimes(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
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

            return Ok(new
            {
                Data = animes,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/shows/mangas
        [HttpGet("mangas")]
        public async Task<ActionResult<IEnumerable<Show>>> GetMangas(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
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

            return Ok(new
            {
                Data = mangas,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            });
        }

        // GET: api/shows/top-rated
        [HttpGet("top-rated")]
        public async Task<ActionResult<IEnumerable<Show>>> GetTopRated(
            [FromQuery] int limit = 10,
            [FromQuery] Tipo? tipo = null)
        {
            var query = _context.Shows
                .AsQueryable();

            if (tipo.HasValue)
            {
                query = query.Where(s => s.Tipo == tipo);
            }

            var topShows = await query
                .OrderByDescending(s => s.Nota)
                .Take(limit)
                .ToListAsync();

            return Ok(topShows);
        }

        // GET: api/shows/trending
        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<Show>>> GetTrending(
            [FromQuery] int limit = 10,
            [FromQuery] string? tipo = null)
        {
            var query = _context.Shows
                .AsQueryable();

            // Ordenar por data de criação mais recente (assumindo que há uma propriedade DataCriacao)
            var trendingShows = await query
                .OrderByDescending(s => s.Views)
                .Take(limit)
                .ToListAsync();

            return Ok(trendingShows);
        }

        // POST: api/shows
        [HttpPost]
        public async Task<ActionResult<Show>> CreateShow(Show show)
        {
            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShow), new { id = show.Id }, show);
        }

        // PUT: api/shows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, Show show)
        {
            if (id != show.Id)
            {
                return BadRequest();
            }

            _context.Entry(show).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/shows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return NoContent();
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
        public async Task<ActionResult<IEnumerable<Genero>>> GetGeneros()
        {
            return await _context.Generos.ToListAsync();
        }

        // GET: api/generos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genero>> GetGenero(int id)
        {
            var genero = await _context.Generos.FindAsync(id);

            if (genero == null)
            {
                return NotFound();
            }

            return genero;
        }

        // GET: api/generos/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<IEnumerable<Show>>> GetShowsByGenero(int id)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genero == null)
            {
                return NotFound();
            }

            return Ok(genero.Shows);
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
        public async Task<ActionResult<IEnumerable<Studio>>> GetStudios()
        {
            return await _context.Studios.ToListAsync();
        }

        // GET: api/studios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Studio>> GetStudio(int id)
        {
            var studio = await _context.Studios.FindAsync(id);

            if (studio == null)
            {
                return NotFound();
            }

            return studio;
        }

        // GET: api/studios/5/shows
        [HttpGet("{id}/shows")]
        public async Task<ActionResult<IEnumerable<Show>>> GetShowsByStudio(int id)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null)
            {
                return NotFound();
            }

            return Ok(studio.ShowsDesenvolvidos);
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
        public async Task<ActionResult<IEnumerable<Personagem>>> GetPersonagens(
            [FromQuery] string? search = null,
            [FromQuery] int? showId = null)
        {
            var query = _context.Personagens
                .Include(p => p.Shows)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nome.Contains(search));
            }

            if (showId.HasValue)
            {
                //query = query.Where(p => p.Shows == showId.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personagem>> GetPersonagem(int id)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null)
            {
                return NotFound();
            }

            return personagem;
        }
    }

    // ================== CONTROLLER DE LISTAS DE UTILIZADORES ==================
    [ApiController]
    [Route("api/[controller]")]
    public class ListasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/listas/utilizador/5
        [HttpGet("utilizador/{utilizadorId}")]
        public async Task<ActionResult<Lista>> GetListaByUtilizador(int utilizadorId)
        {
            var lista = await _context.Listas
                .Include(l => l.ListaShows)
                    .ThenInclude(ls => ls.Show)
                        .ThenInclude(s => s.GenerosShows)
                .Include(l => l.ListaShows)
                    .ThenInclude(ls => ls.Show)
                        .ThenInclude(s => s.Studio)
                .FirstOrDefaultAsync(l => l.UtilizadorId == utilizadorId);

            if (lista == null)
            {
                return NotFound();
            }

            return lista;
        }

        // POST: api/listas/utilizador/5/show/10
        [HttpPost("utilizador/{utilizadorId}/show/{showId}")]
        public async Task<ActionResult> AddShowToLista(int utilizadorId, int showId, [FromBody] ListaStatus status = ListaStatus.Assistir)
        {
            var lista = await _context.Listas
                .FirstOrDefaultAsync(l => l.UtilizadorId == utilizadorId);

            if (lista == null)
            {
                return NotFound("Lista não encontrada");
            }

            var show = await _context.Shows.FindAsync(showId);
            if (show == null)
            {
                return NotFound("Show não encontrado");
            }

            // Verificar se já existe
            var existingEntry = await _context.ListaShows
                .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

            if (existingEntry != null)
            {
                return BadRequest("Show já está na lista");
            }

            var listaShow = new ListaShows
            {
                ListaId = lista.Id,
                ShowId = showId,
                ListaStatus = status,
                //DataAdicao = DateTime.Now
            };

            _context.ListaShows.Add(listaShow);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/listas/utilizador/5/show/10/status
        [HttpPut("utilizador/{utilizadorId}/show/{showId}/status")]
        public async Task<ActionResult> UpdateShowStatus(int utilizadorId, int showId, [FromBody] ListaStatus status)
        {
            var lista = await _context.Listas
                .FirstOrDefaultAsync(l => l.UtilizadorId == utilizadorId);

            if (lista == null)
            {
                return NotFound("Lista não encontrada");
            }

            var listaShow = await _context.ListaShows
                .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

            if (listaShow == null)
            {
                return NotFound("Show não encontrado na lista");
            }

            listaShow.ListaStatus = status;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/listas/utilizador/5/show/10
        [HttpDelete("utilizador/{utilizadorId}/show/{showId}")]
        public async Task<ActionResult> RemoveShowFromLista(int utilizadorId, int showId)
        {
            var lista = await _context.Listas
                .FirstOrDefaultAsync(l => l.UtilizadorId == utilizadorId);

            if (lista == null)
            {
                return NotFound("Lista não encontrada");
            }

            var listaShow = await _context.ListaShows
                .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

            if (listaShow == null)
            {
                return NotFound("Show não encontrado na lista");
            }

            _context.ListaShows.Remove(listaShow);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    // ================== CONTROLLER DE ESTATÍSTICAS ==================
    [ApiController]
    [Route("api/[controller]")]
    public class EstatisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/estatisticas/geral
        [HttpGet("geral")]
        public async Task<ActionResult> GetEstatisticasGerais()
        {
            var totalShows = await _context.Shows.CountAsync();
            var totalAnimes = await _context.Shows.Where(s => s.Tipo == Tipo.Anime).CountAsync();
            var totalMangas = await _context.Shows.Where(s => s.Tipo == Tipo.Manga).CountAsync();
            var totalUtilizadores = await _context.Utilizadores.CountAsync();
            var totalGeneros = await _context.Generos.CountAsync();
            var totalStudios = await _context.Studios.CountAsync();

            return Ok(new
            {
                TotalShows = totalShows,
                TotalAnimes = totalAnimes,
                TotalMangas = totalMangas,
                TotalUtilizadores = totalUtilizadores,
                TotalGeneros = totalGeneros,
                TotalStudios = totalStudios
            });
        }

        // GET: api/estatisticas/generos-populares
        [HttpGet("generos-populares")]
        public async Task<ActionResult> GetGenerosPopulares()
        {
            var generosPopulares = await _context.Generos
                .Select(g => new
                {
                    g.Id,
                    g.GeneroNome,
                    TotalShows = g.Shows.Count()
                })
                .OrderByDescending(g => g.TotalShows)
                .Take(10)
                .ToListAsync();

            return Ok(generosPopulares);
        }
    }
}