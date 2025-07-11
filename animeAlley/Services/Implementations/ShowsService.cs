using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace animeAlley.Services.Implementations
{
    public class ShowsService : IShowsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ShowsService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShowResumoDTO>> BuscarShowsAsync(ShowFiltroDTO filtro)
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
                                       s.Sinopse.Contains(filtro.Search));
            }

            if (filtro.GeneroIds != null && filtro.GeneroIds.Any())
            {
                query = query.Where(s => s.GenerosShows.Any(g => filtro.GeneroIds.Contains(g.Id)));
            }

            if (filtro.StudioIds != null && filtro.StudioIds.Any())
            {
                query = query.Where(s => filtro.StudioIds.Contains(s.Studio.Id));
            }

            if (filtro.AutorIds != null && filtro.AutorIds.Any())
            {
                query = query.Where(s => filtro.AutorIds.Contains(s.Autor.Id));
            }

            if (!string.IsNullOrEmpty(filtro.Status))
            {
                query = query.Where(s => s.Status.ToString() == filtro.Status);
            }

            if (filtro.NotaMinima.HasValue)
            {
                query = query.Where(s => s.Nota >= filtro.NotaMinima);
            }

            if (filtro.NotaMaxima.HasValue)
            {
                query = query.Where(s => s.Nota <= filtro.NotaMaxima);
            }

            if (filtro.AnoInicio.HasValue)
            {
                query = query.Where(s => s.Ano >= filtro.AnoInicio);
            }

            if (filtro.AnoFim.HasValue)
            {
                query = query.Where(s => s.Ano <= filtro.AnoFim);
            }

            if (!string.IsNullOrEmpty(filtro.Fonte))
            {
                query = query.Where(s => s.Fonte.ToString() == filtro.Fonte);
            }

            // Aplicar ordenação
            switch (filtro.OrdenarPor.ToLower())
            {
                case "nota":
                    query = filtro.Direcao.ToUpper() == "DESC" ?
                        query.OrderByDescending(s => s.Nota) :
                        query.OrderBy(s => s.Nota);
                    break;
                case "ano":
                    query = filtro.Direcao.ToUpper() == "DESC" ?
                        query.OrderByDescending(s => s.Ano) :
                        query.OrderBy(s => s.Ano);
                    break;
                case "datacriacao":
                    query = filtro.Direcao.ToUpper() == "DESC" ?
                        query.OrderByDescending(s => s.DataCriacao) :
                        query.OrderBy(s => s.DataCriacao);
                    break;
                default:
                    query = filtro.Direcao.ToUpper() == "DESC" ?
                        query.OrderByDescending(s => s.Nome) :
                        query.OrderBy(s => s.Nome);
                    break;
            }

            // Aplicar paginação
            var shows = await query
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<ShowDetalheDTO?> ObterShowPorIdAsync(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Include(s => s.Personagens)
                .FirstOrDefaultAsync(s => s.Id == id);

            return show == null ? null : _mapper.Map<ShowDetalheDTO>(show);
        }

        public async Task<ShowDetalheDTO> CriarShowAsync(ShowCreateUpdateDTO showDto)
        {
            var show = _mapper.Map<Show>(showDto);
            show.DataCriacao = DateTime.UtcNow;

            // Buscar gêneros
            if (showDto.GeneroIds.Any())
            {
                show.GenerosShows = await _context.Generos
                    .Where(g => showDto.GeneroIds.Contains(g.Id))
                    .ToListAsync();
            }

            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return await ObterShowPorIdAsync(show.Id) ??
                   throw new InvalidOperationException("Erro ao criar show");
        }

        public async Task<ShowDetalheDTO?> AtualizarShowAsync(int id, ShowCreateUpdateDTO showDto)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return null;

            _mapper.Map(showDto, show);
            show.DataAtualizacao = DateTime.UtcNow;

            // Atualizar gêneros
            show.GenerosShows.Clear();
            if (showDto.GeneroIds.Any())
            {
                show.GenerosShows = await _context.Generos
                    .Where(g => showDto.GeneroIds.Contains(g.Id))
                    .ToListAsync();
            }

            await _context.SaveChangesAsync();
            return await ObterShowPorIdAsync(id);
        }

        public async Task<bool> RemoverShowAsync(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show == null) return false;

            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ShowPopularDTO>> ObterShowsPopularesAsync(int limite)
        {
            var shows = await _context.Shows
                .Include(s => s.ListaShows)
                .OrderByDescending(s => s.ListaShows.Count)
                .ThenByDescending(s => s.Nota)
                .Take(limite)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowPopularDTO>>(shows);
        }

        public async Task<IEnumerable<ShowResumoDTO>> ObterResumoShowsAsync(string? search, List<int>? generoIds, int limite)
        {
            var query = _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Nome.Contains(search));
            }

            if (generoIds != null && generoIds.Any())
            {
                query = query.Where(s => s.GenerosShows.Any(g => generoIds.Contains(g.Id)));
            }

            var shows = await query
                .OrderByDescending(s => s.Nota)
                .Take(limite)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<RelatorioShowsDTO> GerarRelatorioShowsAsync(int? inicio, int? fim)
        {
            var query = _context.Shows.AsQueryable();

            if (inicio.HasValue)
            {
                query = query.Where(s => s.Ano >= inicio.Value);
            }
            if (fim.HasValue)
            {
                query = query.Where(s => s.Ano <= fim.Value);
            }

            var shows = await query
                .Include(s => s.GenerosShows)
                .ToListAsync();

            var totalShows = shows.Count;

            // Shows por ano com tratamento seguro de Average
            var showsPorAno = shows
                .GroupBy(s => s.Ano)
                .Select(g => new ShowsPorAnoDTO
                {
                    Ano = g.Key,
                    Quantidade = g.Count(),
                    NotaMedia = g.Where(s => s.Nota > 0)
                                .Select(s => s.Nota)
                                .DefaultIfEmpty(0m)
                                .Average()
                })
                .OrderBy(s => s.Ano)
                .ToList();

            // Shows por gênero com tratamento seguro
            var showsPorGenero = shows
                .SelectMany(s => s.GenerosShows.Select(g => new { Show = s, Genero = g.GeneroNome }))
                .GroupBy(x => x.Genero)
                .Select(g => new ShowsPorGeneroDTO
                {
                    Genero = g.Key,
                    Quantidade = g.Count(),
                    NotaMedia = g.Where(s => s.Show.Nota > 0)
                                .Select(x => x.Show.Nota)
                                .DefaultIfEmpty(0m)
                                .Average()
                })
                .OrderByDescending(s => s.Quantidade)
                .ToList();

            var showsPorStatus = shows
                .GroupBy(s => s.Status)
                .Select(g => new ShowsPorStatusDTO
                {
                    Status = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .OrderByDescending(s => s.Quantidade)
                .ToList();

            return new RelatorioShowsDTO
            {
                TotalShows = totalShows,
                AnoInicio = inicio ?? int.MinValue,
                AnoFim = fim ?? int.MaxValue,
                ShowsPorAno = showsPorAno,
                ShowsPorGenero = showsPorGenero,
                ShowsPorStatus = showsPorStatus
            };
        }

        public async Task<IEnumerable<ShowResumoDTO>> ObterShowsPorAutorAsync(int autorId, int page, int pageSize)
        {
            var shows = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Where(s => s.Autor.Id == autorId)
                .OrderBy(s => s.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<IEnumerable<ShowResumoDTO>> ObterShowsPorStudioAsync(int studioId, int page, int pageSize)
        {
            var shows = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Where(s => s.Studio.Id == studioId)
                .OrderBy(s => s.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<IEnumerable<ShowResumoDTO>> ObterShowsPorGeneroAsync(int generoId, int page, int pageSize)
        {
            var shows = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Where(s => s.GenerosShows.Any(g => g.Id == generoId))
                .OrderBy(s => s.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<IEnumerable<ShowResumoDTO>> ObterShowsPorAnoAsync(int ano, int page, int pageSize)
        {
            var shows = await _context.Shows
                .Include(s => s.GenerosShows)
                .Include(s => s.Studio)
                .Include(s => s.Autor)
                .Where(s => s.Ano == ano)
                .OrderBy(s => s.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowResumoDTO>>(shows);
        }

        public async Task<object> ObterEstatisticasShowsAsync()
        {
            var totalShows = await _context.Shows.CountAsync();
            var notaMedia = await _context.Shows
                .Where(s => s.Nota > 0)
                .AverageAsync(s => s.Nota);

            var showsPorStatus = await _context.Shows
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Quantidade = g.Count() })
                .ToListAsync();

            var generoMaisPopular = await _context.Shows
                .SelectMany(s => s.GenerosShows)
                .GroupBy(g => g.GeneroNome)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Genero = g.Key, Quantidade = g.Count() })
                .FirstOrDefaultAsync();

            return new
            {
                TotalShows = totalShows,
                NotaMedia = notaMedia,
                ShowsPorStatus = showsPorStatus,
                GeneroMaisPopular = generoMaisPopular
            };
        }
    }
}