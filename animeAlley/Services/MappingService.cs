using animeAlley.Data;
using animeAlley.DTOs;
using animeAlley.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace animeAlley.Services
{
    // ================== INTERFACES DOS SERVICES ==================

    public interface IShowService
    {
        Task<PaginatedResponseDto<ShowResumoDto>> GetShowsAsync(ShowFiltroDto filtro);
        Task<ShowDetalheDto?> GetShowByIdAsync(int id);
        Task<PaginatedResponseDto<ShowResumoDto>> GetAnimesAsync(string? search, int page, int pageSize);
        Task<PaginatedResponseDto<ShowResumoDto>> GetMangasAsync(string? search, int page, int pageSize);
        Task<List<ShowResumoDto>> GetTopRatedAsync(int limit, string? tipo);
        Task<List<ShowResumoDto>> GetTrendingAsync(int limit, string? tipo);
        Task<ShowDetalheDto> CreateShowAsync(ShowCreateUpdateDto showDto);
        Task<ShowDetalheDto> UpdateShowAsync(int id, ShowCreateUpdateDto showDto);
        Task<bool> DeleteShowAsync(int id);
        Task<bool> ShowExistsAsync(int id);
    }

    public interface IGeneroService
    {
        Task<List<GeneroDto>> GetGenerosAsync();
        Task<GeneroDto?> GetGeneroByIdAsync(int id);
        Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByGeneroAsync(int generoId, int page, int pageSize);
        Task<GeneroDto> CreateGeneroAsync(GeneroCreateUpdateDto generoDto);
        Task<GeneroDto> UpdateGeneroAsync(int id, GeneroCreateUpdateDto generoDto);
        Task<bool> DeleteGeneroAsync(int id);
        Task<bool> GeneroExistsAsync(int id);
    }

    public interface IStudioService
    {
        Task<PaginatedResponseDto<StudioDto>> GetStudiosAsync(int page, int pageSize, string? search);
        Task<StudioDto?> GetStudioByIdAsync(int id);
        Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByStudioAsync(int studioId, int page, int pageSize);
        Task<StudioDto> CreateStudioAsync(StudioCreateUpdateDto studioDto);
        Task<StudioDto> UpdateStudioAsync(int id, StudioCreateUpdateDto studioDto);
        Task<bool> DeleteStudioAsync(int id);
        Task<bool> StudioExistsAsync(int id);
    }

    public interface IAutorService
    {
        Task<PaginatedResponseDto<AutorDto>> GetAutoresAsync(int page, int pageSize, string? search);
        Task<AutorDto?> GetAutorByIdAsync(int id);
        Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByAutorAsync(int autorId, int page, int pageSize);
        Task<AutorDto> CreateAutorAsync(AutorCreateUpdateDto autorDto);
        Task<AutorDto> UpdateAutorAsync(int id, AutorCreateUpdateDto autorDto);
        Task<bool> DeleteAutorAsync(int id);
        Task<bool> AutorExistsAsync(int id);
    }

    public interface IPersonagemService
    {
        Task<PaginatedResponseDto<PersonagemDto>> GetPersonagensAsync(int page, int pageSize, string? search);
        Task<PersonagemDto?> GetPersonagemByIdAsync(int id);
        Task<PersonagemDto> CreatePersonagemAsync(PersonagemCreateUpdateDto personagemDto);
        Task<PersonagemDto> UpdatePersonagemAsync(int id, PersonagemCreateUpdateDto personagemDto);
        Task<bool> DeletePersonagemAsync(int id);
        Task<bool> PersonagemExistsAsync(int id);
    }

    public interface IEstatisticasService
    {
        Task<EstatisticasDto> GetEstatisticasGeraisAsync();
        Task<RelatorioShowsDto> GetRelatorioShowsAsync(DateTime? inicio, DateTime? fim);
    }

    // ================== IMPLEMENTAÇÃO DOS SERVICES ==================

    // ================== SHOW SERVICE ==================
    public class ShowService : IShowService
    {
        private readonly ApplicationDbContext _context;

        public ShowService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetShowsAsync(ShowFiltroDto filtro)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtro.Search))
            {
                query = query.Where(s => s.Nome.Contains(filtro.Search) ||
                                       s.Sinopse != null && s.Sinopse.Contains(filtro.Search));
            }

            if (filtro.GeneroIds?.Any() == true)
            {
                query = query.Where(s => s.GenerosShows.Any(g => filtro.GeneroIds.Contains(g.Id)));
            }

            if (filtro.StudioIds?.Any() == true)
            {
                query = query.Where(s => filtro.StudioIds.Contains(s.StudioFK));
            }

            if (filtro.AutorIds?.Any() == true)
            {
                query = query.Where(s => filtro.AutorIds.Contains(s.AutorFK));
            }

            if (!string.IsNullOrEmpty(filtro.Status))
            {
                if (Enum.TryParse<Status>(filtro.Status, out var status))
                {
                    query = query.Where(s => s.Status == status);
                }
            }

            if (filtro.NotaMinima.HasValue)
            {
                query = query.Where(s => s.Nota >= filtro.NotaMinima.Value);
            }

            if (filtro.NotaMaxima.HasValue)
            {
                query = query.Where(s => s.Nota <= filtro.NotaMaxima.Value);
            }

            if (filtro.AnoInicio.HasValue)
            {
                query = query.Where(s => s.Ano >= filtro.AnoInicio.Value);
            }

            if (filtro.AnoFim.HasValue)
            {
                query = query.Where(s => s.Ano <= filtro.AnoFim.Value);
            }

            if (!string.IsNullOrEmpty(filtro.Fonte))
            {
                if (Enum.TryParse<Fonte>(filtro.Fonte, out var fonte))
                {
                    query = query.Where(s => s.Fonte == fonte);
                }
            }

            // Ordenação
            query = filtro.OrdenarPor.ToLower() switch
            {
                "nota" => filtro.Direcao.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Nota) : query.OrderBy(s => s.Nota),
                "ano" => filtro.Direcao.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Ano) : query.OrderBy(s => s.Ano),
                "datacriacao" => filtro.Direcao.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id),
                _ => filtro.Direcao.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Nome) : query.OrderBy(s => s.Nome)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)filtro.PageSize);

            var shows = await query
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            var showDtos = shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = s.Studio?.Nome ?? string.Empty,
                Autor = s.Autor?.Nome ?? string.Empty
            }).ToList();

            return new PaginatedResponseDto<ShowResumoDto>
            {
                Data = showDtos,
                CurrentPage = filtro.Page,
                PageSize = filtro.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<ShowDetalheDto?> GetShowByIdAsync(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Include(s => s.Personagens)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return null;

            // Incrementar views
            show.Views++;
            await _context.SaveChangesAsync();

            return new ShowDetalheDto
            {
                Id = show.Id,
                Nome = show.Nome,
                Sinopse = show.Sinopse,
                Imagem = show.Imagem,
                Banner = show.Banner,
                Trailer = show.Trailer,
                Nota = show.Nota,
                Ano = show.Ano,
                Status = show.Status.ToString(),
                Fonte = show.Fonte.ToString(),
                Generos = show.GenerosShows.Select(g => new GeneroDto
                {
                    Id = g.Id,
                    Nome = g.GeneroNome
                }).ToList(),
                Studio = new StudioDto
                {
                    Id = show.Studio?.Id ?? 0,
                    Nome = show.Studio?.Nome ?? string.Empty,
                    Foto = show.Studio?.Foto,
                    Sobre = show.Studio?.Sobre,
                    Fundado = show.Studio?.Fundado,
                    Fechado = show.Studio?.Fechado,
                    Status = show.Studio?.Status.ToString() ?? string.Empty
                },
                Autor = new AutorDto
                {
                    Id = show.Autor?.Id ?? 0,
                    Nome = show.Autor?.Nome ?? string.Empty,
                    Biografia = show.Autor?.Sobre,
                    Foto = show.Autor?.Foto,
                    DataNascimento = show.Autor?.DateNasc
                },
                Personagens = show.Personagens?.Select(p => new PersonagemResumoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    TipoPersonagem = p.TipoPersonagem.ToString(),
                    Foto = p.Foto
                }).ToList() ?? new List<PersonagemResumoDto>()
            };
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetAnimesAsync(string? search, int page, int pageSize)
        {
            var filtro = new ShowFiltroDto
            {
                Search = search,
                Tipo = "Anime",
                Page = page,
                PageSize = pageSize
            };

            return await GetShowsAsync(filtro);
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetMangasAsync(string? search, int page, int pageSize)
        {
            var filtro = new ShowFiltroDto
            {
                Search = search,
                Tipo = "Manga",
                Page = page,
                PageSize = pageSize
            };

            return await GetShowsAsync(filtro);
        }

        public async Task<List<ShowResumoDto>> GetTopRatedAsync(int limit, string? tipo)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .AsQueryable();

            var shows = await query
                .OrderByDescending(s => s.Nota)
                .Take(limit)
                .ToListAsync();

            return shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = s.Studio?.Nome ?? string.Empty,
                Autor = s.Autor?.Nome ?? string.Empty
            }).ToList();
        }

        public async Task<List<ShowResumoDto>> GetTrendingAsync(int limit, string? tipo)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .AsQueryable();

            var shows = await query
                .OrderByDescending(s => s.Views)
                .Take(limit)
                .ToListAsync();

            return shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = s.Studio?.Nome ?? string.Empty,
                Autor = s.Autor?.Nome ?? string.Empty
            }).ToList();
        }

        public async Task<ShowDetalheDto> CreateShowAsync(ShowCreateUpdateDto showDto)
        {
            // Validar se Studio e Autor existem
            var studio = await _context.Studios.FindAsync(showDto.StudioId);
            if (studio == null)
                throw new ValidationException("Studio não encontrado");

            var autor = await _context.Autores.FindAsync(showDto.AutorId);
            if (autor == null)
                throw new ValidationException("Autor não encontrado");

            // Validar gêneros
            var generos = new List<Genero>();
            if (showDto.GeneroIds.Any())
            {
                generos = await _context.Generos
                    .Where(g => showDto.GeneroIds.Contains(g.Id))
                    .ToListAsync();

                if (generos.Count != showDto.GeneroIds.Count)
                {
                    var foundIds = generos.Select(g => g.Id);
                    var notFoundIds = showDto.GeneroIds.Except(foundIds);
                    throw new ValidationException($"Gêneros não encontrados: {string.Join(", ", notFoundIds)}");
                }
            }

            var show = new Show
            {
                Nome = showDto.Nome,
                Sinopse = showDto.Sinopse,
                Status = Enum.Parse<Status>(showDto.Status),
                Nota = showDto.Nota ?? 0,
                Ano = showDto.Ano,
                Imagem = showDto.Imagem ?? string.Empty,
                Banner = showDto.Banner ?? string.Empty,
                Trailer = showDto.Trailer ?? string.Empty,
                Fonte = Enum.Parse<Fonte>(showDto.Fonte),
                StudioFK = showDto.StudioId,
                AutorFK = showDto.AutorId,
                Views = 0,
                GenerosShows = generos
            };

            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return await GetShowByIdAsync(show.Id) ?? throw new InvalidOperationException("Erro ao recuperar show criado");
        }

        public async Task<ShowDetalheDto> UpdateShowAsync(int id, ShowCreateUpdateDto showDto)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
                throw new ValidationException("Show não encontrado");

            // Validar se Studio e Autor existem
            var studio = await _context.Studios.FindAsync(showDto.StudioId);
            if (studio == null)
                throw new ValidationException("Studio não encontrado");

            var autor = await _context.Autores.FindAsync(showDto.AutorId);
            if (autor == null)
                throw new ValidationException("Autor não encontrado");

            // Atualizar propriedades
            show.Nome = showDto.Nome;
            show.Sinopse = showDto.Sinopse;
            show.Status = Enum.Parse<Status>(showDto.Status);
            show.Nota = showDto.Nota ?? show.Nota;
            show.Ano = showDto.Ano;
            show.Imagem = showDto.Imagem ?? show.Imagem;
            show.Banner = showDto.Banner ?? show.Banner;
            show.Trailer = showDto.Trailer ?? show.Trailer;
            show.Fonte = Enum.Parse<Fonte>(showDto.Fonte);
            show.StudioFK = showDto.StudioId;
            show.AutorFK = showDto.AutorId;

            // Atualizar gêneros
            show.GenerosShows.Clear();
            if (showDto.GeneroIds.Any())
            {
                var generos = await _context.Generos
                    .Where(g => showDto.GeneroIds.Contains(g.Id))
                    .ToListAsync();

                if (generos.Count != showDto.GeneroIds.Count)
                {
                    var foundIds = generos.Select(g => g.Id);
                    var notFoundIds = showDto.GeneroIds.Except(foundIds);
                    throw new ValidationException($"Gêneros não encontrados: {string.Join(", ", notFoundIds)}");
                }

                show.GenerosShows = generos;
            }

            await _context.SaveChangesAsync();

            return await GetShowByIdAsync(id) ?? throw new InvalidOperationException("Erro ao recuperar show atualizado");
        }

        public async Task<bool> DeleteShowAsync(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return false;

            show.GenerosShows.Clear();
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ShowExistsAsync(int id)
        {
            return await _context.Shows.AnyAsync(e => e.Id == id);
        }
    }

    // ================== GENERO SERVICE ==================
    public class GeneroService : IGeneroService
    {
        private readonly ApplicationDbContext _context;

        public GeneroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GeneroDto>> GetGenerosAsync()
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .ToListAsync();

            return generos.Select(g => new GeneroDto
            {
                Id = g.Id,
                Nome = g.GeneroNome,
                TotalShows = g.Shows?.Count ?? 0
            }).ToList();
        }

        public async Task<GeneroDto?> GetGeneroByIdAsync(int id)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genero == null) return null;

            return new GeneroDto
            {
                Id = genero.Id,
                Nome = genero.GeneroNome,
                TotalShows = genero.Shows?.Count ?? 0
            };
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByGeneroAsync(int generoId, int page, int pageSize)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                    .ThenInclude(s => s.Studio)
                .Include(g => g.Shows)
                    .ThenInclude(s => s.Autor)
                .Include(g => g.Shows)
                    .ThenInclude(s => s.GenerosShows)
                .FirstOrDefaultAsync(g => g.Id == generoId);

            if (genero == null)
                throw new ValidationException("Gênero não encontrado");

            var totalItems = genero.Shows?.Count ?? 0;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var shows = genero.Shows?
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Show>();

            var showDtos = shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = s.Studio?.Nome ?? string.Empty,
                Autor = s.Autor?.Nome ?? string.Empty
            }).ToList();

            return new PaginatedResponseDto<ShowResumoDto>
            {
                Data = showDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<GeneroDto> CreateGeneroAsync(GeneroCreateUpdateDto generoDto)
        {
            var genero = new Genero
            {
                GeneroNome = generoDto.Nome
            };

            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();

            return new GeneroDto
            {
                Id = genero.Id,
                Nome = genero.GeneroNome,
                TotalShows = 0
            };
        }

        public async Task<GeneroDto> UpdateGeneroAsync(int id, GeneroCreateUpdateDto generoDto)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genero == null)
                throw new ValidationException("Gênero não encontrado");

            genero.GeneroNome = generoDto.Nome;
            await _context.SaveChangesAsync();

            return new GeneroDto
            {
                Id = genero.Id,
                Nome = genero.GeneroNome,
                TotalShows = genero.Shows?.Count ?? 0
            };
        }

        public async Task<bool> DeleteGeneroAsync(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero == null) return false;

            _context.Generos.Remove(genero);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GeneroExistsAsync(int id)
        {
            return await _context.Generos.AnyAsync(e => e.Id == id);
        }
    }

    // ================== STUDIO SERVICE ==================
    public class StudioService : IStudioService
    {
        private readonly ApplicationDbContext _context;

        public StudioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<StudioDto>> GetStudiosAsync(int page, int pageSize, string? search)
        {
            var query = _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Nome.Contains(search) ||
                                       s.Sobre != null && s.Sobre.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var studios = await query
                .OrderBy(s => s.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studioDtos = studios.Select(s => new StudioDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Foto = s.Foto,
                Sobre = s.Sobre,
                Fundado = s.Fundado,
                Fechado = s.Fechado,
                Status = s.Status.ToString(),
                TotalShows = s.ShowsDesenvolvidos?.Count ?? 0
            }).ToList();

            return new PaginatedResponseDto<StudioDto>
            {
                Data = studioDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<StudioDto?> GetStudioByIdAsync(int id)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null) return null;

            return new StudioDto
            {
                Id = studio.Id,
                Nome = studio.Nome,
                Foto = studio.Foto,
                Sobre = studio.Sobre,
                Fundado = studio.Fundado,
                Fechado = studio.Fechado,
                Status = studio.Status.ToString(),
                TotalShows = studio.ShowsDesenvolvidos?.Count ?? 0
            };
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByStudioAsync(int studioId, int page, int pageSize)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                    .ThenInclude(show => show.GenerosShows)
                .Include(s => s.ShowsDesenvolvidos)
                    .ThenInclude(show => show.Autor)
                .FirstOrDefaultAsync(s => s.Id == studioId);

            if (studio == null)
                throw new ValidationException("Studio não encontrado");

            var totalItems = studio.ShowsDesenvolvidos?.Count ?? 0;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var shows = studio.ShowsDesenvolvidos?
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Show>();

            var showDtos = shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = studio.Nome,
                Autor = s.Autor?.Nome ?? string.Empty
            }).ToList();

            return new PaginatedResponseDto<ShowResumoDto>
            {
                Data = showDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<StudioDto> CreateStudioAsync(StudioCreateUpdateDto studioDto)
        {
            var studio = new Studio
            {
                Nome = studioDto.Nome,
                Foto = studioDto.Foto,
                Sobre = studioDto.Sobre,
                Fundado = studioDto.Fundado,
                Fechado = studioDto.Fechado,
                Status = Enum.Parse<Estado>(studioDto.Status)
            };

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();

            return new StudioDto
            {
                Id = studio.Id,
                Nome = studio.Nome,
                Foto = studio.Foto,
                Sobre = studio.Sobre,
                Fundado = studio.Fundado,
                Fechado = studio.Fechado,
                Status = studio.Status.ToString(),
                TotalShows = 0
            };
        }

        public async Task<StudioDto> UpdateStudioAsync(int id, StudioCreateUpdateDto studioDto)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null)
                throw new ValidationException("Studio não encontrado");

            studio.Nome = studioDto.Nome;
            studio.Foto = studioDto.Foto;
            studio.Sobre = studioDto.Sobre;
            studio.Fundado = studioDto.Fundado;
            studio.Fechado = studioDto.Fechado;
            studio.Status = Enum.Parse<Estado>(studioDto.Status);

            await _context.SaveChangesAsync();

            return new StudioDto
            {
                Id = studio.Id,
                Nome = studio.Nome,
                Foto = studio.Foto,
                Sobre = studio.Sobre,
                Fundado = studio.Fundado,
                Fechado = studio.Fechado,
                Status = studio.Status.ToString(),
                TotalShows = studio.ShowsDesenvolvidos?.Count ?? 0
            };
        }

        public async Task<bool> DeleteStudioAsync(int id)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null) return false;

            // Verificar se o studio tem shows associados
            if (studio.ShowsDesenvolvidos?.Any() == true)
            {
                throw new ValidationException("Não é possível excluir um studio que possui shows associados");
            }

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StudioExistsAsync(int id)
        {
            return await _context.Studios.AnyAsync(s => s.Id == id);
        }
    }

    // ================== AUTOR SERVICE ==================
    public class AutorService : IAutorService
    {
        private readonly ApplicationDbContext _context;

        public AutorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<AutorDto>> GetAutoresAsync(int page, int pageSize, string? search)
        {
            var query = _context.Autores
                .Include(a => a.ShowsCriados)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Nome.Contains(search) ||
                                       a.Sobre != null && a.Sobre.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var autores = await query
                .OrderBy(a => a.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var autorDtos = autores.Select(a => new AutorDto
            {
                Id = a.Id,
                Nome = a.Nome,
                Biografia = a.Sobre,
                Foto = a.Foto,
                DataNascimento = a.DateNasc,
                TotalObras = a.ShowsCriados?.Count ?? 0
            }).ToList();

            return new PaginatedResponseDto<AutorDto>
            {
                Data = autorDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<AutorDto?> GetAutorByIdAsync(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null) return null;

            return new AutorDto
            {
                Id = autor.Id,
                Nome = autor.Nome,
                Biografia = autor.Sobre,
                Foto = autor.Foto,
                DataNascimento = autor.DateNasc,
                TotalObras = autor.ShowsCriados?.Count ?? 0
            };
        }

        public async Task<PaginatedResponseDto<ShowResumoDto>> GetShowsByAutorAsync(int autorId, int page, int pageSize)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                    .ThenInclude(show => show.GenerosShows)
                .Include(a => a.ShowsCriados)
                    .ThenInclude(show => show.Studio)
                .FirstOrDefaultAsync(a => a.Id == autorId);

            if (autor == null)
                throw new ValidationException("Autor não encontrado");

            var totalItems = autor.ShowsCriados?.Count ?? 0;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var shows = autor.ShowsCriados?
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Show>();

            var showDtos = shows.Select(s => new ShowResumoDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Sinopse = s.Sinopse,
                Imagem = s.Imagem,
                Nota = s.Nota,
                Ano = s.Ano,
                Status = s.Status.ToString(),
                Generos = s.GenerosShows.Select(g => g.GeneroNome).ToList(),
                Studio = s.Studio?.Nome ?? string.Empty,
                Autor = autor.Nome
            }).ToList();

            return new PaginatedResponseDto<ShowResumoDto>
            {
                Data = showDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<AutorDto> CreateAutorAsync(AutorCreateUpdateDto autorDto)
        {
            var autor = new Autor
            {
                Nome = autorDto.Nome,
                Sobre = autorDto.Biografia,
                Foto = autorDto.Foto,
                DateNasc = autorDto.DataNascimento
            };

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return new AutorDto
            {
                Id = autor.Id,
                Nome = autor.Nome,
                Biografia = autor.Sobre,
                Foto = autor.Foto,
                DataNascimento = autor.DateNasc,
                TotalObras = 0
            };
        }

        public async Task<AutorDto> UpdateAutorAsync(int id, AutorCreateUpdateDto autorDto)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null)
                throw new ValidationException("Autor não encontrado");

            autor.Nome = autorDto.Nome;
            autor.Sobre = autorDto.Biografia;
            autor.Foto = autorDto.Foto;
            autor.DateNasc = autorDto.DataNascimento;

            await _context.SaveChangesAsync();

            return new AutorDto
            {
                Id = autor.Id,
                Nome = autor.Nome,
                Biografia = autor.Sobre,
                Foto = autor.Foto,
                DataNascimento = autor.DateNasc,
                TotalObras = autor.ShowsCriados?.Count ?? 0
            };
        }

        public async Task<bool> DeleteAutorAsync(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null) return false;

            // Verificar se o autor tem shows associados
            if (autor.ShowsCriados?.Any() == true)
            {
                throw new ValidationException("Não é possível excluir um autor que possui shows associados");
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AutorExistsAsync(int id)
        {
            return await _context.Autores.AnyAsync(a => a.Id == id);
        }
    }

    // ================== PERSONAGEM SERVICE ==================
    public class PersonagemService : IPersonagemService
    {
        private readonly ApplicationDbContext _context;

        public PersonagemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<PersonagemDto>> GetPersonagensAsync(int page, int pageSize, string? search)
        {
            var query = _context.Personagens
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.Studio)
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.Autor)
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.GenerosShows)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nome.Contains(search) ||
                                       p.Sinopse != null && p.Sinopse.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var personagens = await query
                .OrderBy(p => p.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var personagemDtos = personagens.Select(p => new PersonagemDto
            {
                Id = p.Id,
                Nome = p.Nome,
                TipoPersonagem = p.TipoPersonagem.ToString(),
                Sexualidade = p.PersonagemSexualidade?.ToString(),
                Idade = p.Idade,
                DataNascimento = p.DataNasc,
                Sinopse = p.Sinopse ?? string.Empty,
                Foto = p.Foto ?? string.Empty,
                Shows = p.Shows?.Select(ps => new ShowResumoDto
                {
                    Id = ps.Id,
                    Nome = ps.Nome,
                    Sinopse = ps.Sinopse,
                    Imagem = ps.Imagem,
                    Nota = ps.Nota,
                    Ano = ps.Ano,
                    Status = ps.Status.ToString(),
                    Generos = ps.GenerosShows.Select(g => g.GeneroNome).ToList(),
                    Studio = ps.Studio?.Nome ?? string.Empty,
                    Autor = ps.Autor?.Nome ?? string.Empty
                }).ToList() ?? new List<ShowResumoDto>()
            }).ToList();

            return new PaginatedResponseDto<PersonagemDto>
            {
                Data = personagemDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<PersonagemDto?> GetPersonagemByIdAsync(int id)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.Studio)
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.Autor)
                .Include(p => p.Shows)
                    .ThenInclude(ps => ps.GenerosShows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null) return null;

            return new PersonagemDto
            {
                Id = personagem.Id,
                Nome = personagem.Nome,
                TipoPersonagem = personagem.TipoPersonagem.ToString(),
                Sexualidade = personagem.PersonagemSexualidade?.ToString(),
                Idade = personagem.Idade,
                DataNascimento = personagem.DataNasc,
                Sinopse = personagem.Sinopse ?? string.Empty,
                Foto = personagem.Foto ?? string.Empty,
                Shows = personagem.Shows?.Select(ps => new ShowResumoDto
                {
                    Id = ps.Id,
                    Nome = ps.Nome,
                    Sinopse = ps.Sinopse,
                    Imagem = ps.Imagem,
                    Nota = ps.Nota,
                    Ano = ps.Ano,
                    Status = ps.Status.ToString(),
                    Generos = ps.GenerosShows.Select(g => g.GeneroNome).ToList(),
                    Studio = ps.Studio?.Nome ?? string.Empty,
                    Autor = ps.Autor?.Nome ?? string.Empty
                }).ToList() ?? new List<ShowResumoDto>()
            };
        }

        public async Task<PersonagemDto> CreatePersonagemAsync(PersonagemCreateUpdateDto personagemDto)
        {
            var personagem = new Personagem
            {
                Nome = personagemDto.Nome,
                TipoPersonagem = Enum.Parse<TiposPersonagem>(personagemDto.TipoPersonagem),
                PersonagemSexualidade = !string.IsNullOrEmpty(personagemDto.Sexualidade)
                    ? Enum.Parse<Sexualidade>(personagemDto.Sexualidade)
                    : null,
                Idade = personagemDto.Idade,
                DataNasc = personagemDto.DataNascimento,
                Sinopse = personagemDto.Sinopse,
                Foto = personagemDto.Foto
            };

            _context.Personagens.Add(personagem);
            await _context.SaveChangesAsync();

            // Associar shows se fornecidos - CORRIGIDO
            if (personagemDto.ShowIds?.Any() == true)
            {
                var shows = await _context.Shows
                    .Where(s => personagemDto.ShowIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var show in shows)
                {
                    personagem.Shows.Add(show);
                }

                await _context.SaveChangesAsync();
            }

            return new PersonagemDto
            {
                Id = personagem.Id,
                Nome = personagem.Nome,
                TipoPersonagem = personagem.TipoPersonagem.ToString(),
                Sexualidade = personagem.PersonagemSexualidade?.ToString(),
                Idade = personagem.Idade,
                DataNascimento = personagem.DataNasc,
                Sinopse = personagem.Sinopse ?? string.Empty,
                Foto = personagem.Foto ?? string.Empty,
                Shows = new List<ShowResumoDto>()
            };
        }

        public async Task<PersonagemDto> UpdatePersonagemAsync(int id, PersonagemCreateUpdateDto personagemDto)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null)
                throw new ValidationException("Personagem não encontrado");

            personagem.Nome = personagemDto.Nome;
            personagem.TipoPersonagem = Enum.Parse<TiposPersonagem>(personagemDto.TipoPersonagem);
            personagem.PersonagemSexualidade = !string.IsNullOrEmpty(personagemDto.Sexualidade)
                ? Enum.Parse<Sexualidade>(personagemDto.Sexualidade)
                : null;
            personagem.Idade = personagemDto.Idade;
            personagem.DataNasc = personagemDto.DataNascimento;
            personagem.Sinopse = personagemDto.Sinopse;
            personagem.Foto = personagemDto.Foto;

            // Atualizar associações com shows - CORRIGIDO
            if (personagemDto.ShowIds != null)
            {
                // Limpar associações existentes
                personagem.Shows.Clear();

                // Adicionar novas associações
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
            }

            await _context.SaveChangesAsync();

            // Recarregar com os dados atualizados
            var personagemAtualizado = await GetPersonagemByIdAsync(id);
            return personagemAtualizado!;
        }

        public async Task<bool> DeletePersonagemAsync(int id)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null) return false;

            // Remover associações primeiro
            _context.Shows.RemoveRange(personagem.Shows);

            // Remover personagem
            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PersonagemExistsAsync(int id)
        {
            return await _context.Personagens.AnyAsync(p => p.Id == id);
        }
    }

    // ================== ESTATÍSTICAS SERVICE ==================
    public class EstatisticasService : IEstatisticasService
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EstatisticasDto> GetEstatisticasGeraisAsync()
        {
            var totalShows = await _context.Shows.CountAsync();
            var totalUtilizadores = await _context.Users.CountAsync();
            var totalGeneros = await _context.Generos.CountAsync();
            var totalStudios = await _context.Studios.CountAsync();
            var totalAutores = await _context.Autores.CountAsync();
            var totalPersonagens = await _context.Personagens.CountAsync();

            var notaMediaGeral = await _context.Shows
                .Where(s => s.Nota > 0)
                .AverageAsync(s => s.Nota);

            // Gêneros populares - CORRIGIDO
            var generosPopulares = await _context.Generos
                .Include(g => g.Shows)
                .Select(g => new GeneroPopularDto
                {
                    Id = g.Id,
                    Nome = g.GeneroNome,
                    TotalShows = g.Shows.Count(),
                    NotaMedia = g.Shows.Any(s => s.Nota > 0)
                        ? g.Shows.Where(s => s.Nota > 0).Average(s => s.Nota)
                        : 0
                })
                .OrderByDescending(g => g.TotalShows)
                .Take(10)
                .ToListAsync();

            // Shows populares (por nota)
            var showsPopulares = await _context.Shows
                .Where(s => s.Nota > 0)
                .OrderByDescending(s => s.Nota)
                .Take(10)
                .Select(s => new ShowPopularDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Nota = s.Nota,
                    TotalNasListas = 0, // Seria necessário implementar sistema de listas de usuário
                    Imagem = s.Imagem
                })
                .ToListAsync();

            // Studios populares
            var studiosPopulares = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .Where(s => s.ShowsDesenvolvidos.Any())
                .Select(s => new StudioPopularDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    TotalShows = s.ShowsDesenvolvidos.Count(),
                    NotaMedia = s.ShowsDesenvolvidos.Any(show => show.Nota > 0)
                        ? s.ShowsDesenvolvidos.Where(show => show.Nota > 0).Average(show => show.Nota)
                        : 0
                })
                .OrderByDescending(s => s.TotalShows)
                .Take(10)
                .ToListAsync();

            return new EstatisticasDto
            {
                TotalShows = totalShows,
                TotalUtilizadores = totalUtilizadores,
                TotalGeneros = totalGeneros,
                TotalStudios = totalStudios,
                TotalAutores = totalAutores,
                TotalPersonagens = totalPersonagens,
                NotaMediaGeral = notaMediaGeral,
                GenerosPopulares = generosPopulares,
                ShowsPopulares = showsPopulares,
                StudiosPopulares = studiosPopulares
            };
        }

        public async Task<RelatorioShowsDto> GetRelatorioShowsAsync(DateTime? inicio, DateTime? fim)
        {
            var query = _context.Shows.AsQueryable();

            if (inicio.HasValue && fim.HasValue)
            {
                query = query.Where(s => s.DataCriacao >= inicio.Value && s.DataCriacao <= fim.Value);
            }

            var totalShows = await query.CountAsync();

            // Shows por ano
            var showsPorAno = await query
                .GroupBy(s => s.Ano)
                .Select(g => new ShowsPorAnoDto
                {
                    Ano = g.Key,
                    Quantidade = g.Count(),
                    NotaMedia = g.Any(s => s.Nota > 0)
                        ? g.Where(s => s.Nota > 0).Average(s => s.Nota)
                        : 0
                })
                .OrderBy(s => s.Ano)
                .ToListAsync();

            // Shows por gênero - CORRIGIDO
            var showsPorGenero = await _context.Generos
                .Include(g => g.Shows)
                .Where(g => g.Shows.Any(s => query.Contains(s)))
                .Select(g => new ShowsPorGeneroDto
                {
                    Genero = g.GeneroNome,
                    Quantidade = g.Shows.Count(s => query.Contains(s)),
                    NotaMedia = g.Shows.Any(s => query.Contains(s) && s.Nota > 0)
                        ? g.Shows.Where(s => query.Contains(s) && s.Nota > 0).Average(s => s.Nota)
                        : 0
                })
                .OrderByDescending(g => g.Quantidade)
                .ToListAsync();

            // Shows por status
            var showsPorStatus = await query
                .GroupBy(s => s.Status)
                .Select(g => new ShowsPorStatusDto
                {
                    Status = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync();

            return new RelatorioShowsDto
            {
                TotalShows = totalShows,
                PeriodoInicio = inicio ?? DateTime.MinValue,
                PeriodoFim = fim ?? DateTime.MaxValue,
                ShowsPorAno = showsPorAno,
                ShowsPorGenero = showsPorGenero,
                ShowsPorStatus = showsPorStatus
            };
        }
    }
}