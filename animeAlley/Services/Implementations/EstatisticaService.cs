using AutoMapper;
using animeAlley.Data;
using animeAlley.Models.ViewModels.EstatisticasDTO;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;
using animeAlley.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class EstatisticaService : IEstatisticaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EstatisticaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EstatisticaDTO> GetEstatisticasCompletasAsync()
        {
            var totalShows = await GetTotalShowsAsync();
            var totalUtilizadores = await GetTotalUtilizadoresAsync();
            var totalGeneros = await GetTotalGenerosAsync();
            var totalStudios = await GetTotalStudiosAsync();
            var totalAutores = await GetTotalAutoresAsync();
            var totalPersonagens = await GetTotalPersonagensAsync();
            var notaMediaGeral = await GetNotaMediaGeralAsync();
            var generosPopulares = await GetGenerosPopularesAsync();
            var showsPopulares = await GetShowsPopularesAsync();
            var studiosPopulares = await GetStudiosPopularesAsync();

            return new EstatisticaDTO
            {
                TotalShows = totalShows,
                TotalUtilizadores = totalUtilizadores,
                TotalGeneros = totalGeneros,
                TotalStudios = totalStudios,
                TotalAutores = totalAutores,
                TotalPersonagens = totalPersonagens,
                NotaMediaGeral = notaMediaGeral,
                GenerosPopulares = generosPopulares.ToList(),
                ShowsPopulares = showsPopulares.ToList(),
                StudiosPopulares = studiosPopulares.ToList()
            };
        }

        public async Task<EstatisticaResumoDTO> GetEstatisticasResumoAsync()
        {
            var totalShows = await GetTotalShowsAsync();
            var totalUtilizadores = await GetTotalUtilizadoresAsync();
            var totalPersonagens = await GetTotalPersonagensAsync();
            var notaMediaGeral = await GetNotaMediaGeralAsync();

            var generoMaisPopular = await _context.Generos
                .Include(g => g.Shows)
                .OrderByDescending(g => g.Shows.Count)
                .Select(g => g.GeneroNome)
                .FirstOrDefaultAsync() ?? "N/A";

            var showMaisPopular = await _context.Shows
                .Include(s => s.Nota)
                .OrderByDescending(s => s.Nota)
                .Select(s => s.Nome)
                .FirstOrDefaultAsync() ?? "N/A";

            return new EstatisticaResumoDTO
            {
                TotalShows = totalShows,
                TotalUtilizadores = totalUtilizadores,
                TotalPersonagens = totalPersonagens,
                NotaMediaGeral = (double)notaMediaGeral,
                GeneroMaisPopular = generoMaisPopular,
                ShowMaisPopular = showMaisPopular
            };
        }

        public async Task<int> GetTotalShowsAsync()
        {
            return await _context.Shows.CountAsync();
        }

        public async Task<int> GetTotalUtilizadoresAsync()
        {
            return await _context.Utilizadores.CountAsync();
        }

        public async Task<int> GetTotalGenerosAsync()
        {
            return await _context.Generos.CountAsync();
        }

        public async Task<int> GetTotalStudiosAsync()
        {
            return await _context.Studios.CountAsync();
        }

        public async Task<int> GetTotalAutoresAsync()
        {
            return await _context.Autores.CountAsync();
        }

        public async Task<int> GetTotalPersonagensAsync()
        {
            return await _context.Personagens.CountAsync();
        }

        public async Task<decimal> GetNotaMediaGeralAsync()
        {
            var media = await _context.Shows
                .Where(a => a.Nota > 0)
                .AverageAsync(a => a.Nota);
            return Math.Round((decimal)media, 2);
        }

        public async Task<IEnumerable<GeneroPopularDTO>> GetGenerosPopularesAsync(int take = 10)
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .OrderByDescending(g => g.Shows.Count)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GeneroPopularDTO>>(generos);
        }

        public async Task<IEnumerable<ShowPopularDTO>> GetShowsPopularesAsync(int take = 10)
        {
            var shows = await _context.Shows
                .OrderByDescending(s => s.Nota)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShowPopularDTO>>(shows);
        }

        public async Task<IEnumerable<StudioPopularDTO>> GetStudiosPopularesAsync(int take = 10)
        {
            var studios = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .OrderByDescending(s => s.ShowsDesenvolvidos.Count)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudioPopularDTO>>(studios);
        }

        public async Task<EstatisticaDTO> GetEstatisticasPorAnoAsync(int ano)
        {
            var showsDoAno = _context.Shows
                .Where(s => s.Ano > 0 && s.Ano == ano);

            var totalShows = await showsDoAno.CountAsync();

            var notaMedia = await showsDoAno
                .Where(a => a.Nota > 0)
                .AverageAsync(a => a.Nota);

            return new EstatisticaDTO
            {
                TotalShows = totalShows,
                NotaMediaGeral = Math.Round((decimal)notaMedia, 2)
            };
        }
    }
}