using AutoMapper;
using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.GenerosDTO;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class GeneroService : IGeneroService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GeneroService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GeneroDTO>> GetAllGenerosAsync()
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GeneroDTO>>(generos);
        }

        public async Task<GeneroDTO?> GetGeneroByIdAsync(int id)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                .FirstOrDefaultAsync(g => g.Id == id);

            return genero == null ? null : _mapper.Map<GeneroDTO>(genero);
        }

        public async Task<GeneroDTO> CreateGeneroAsync(GeneroCreateUpdateDTO generoDto)
        {
            var genero = _mapper.Map<Genero>(generoDto);

            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();

            // Recarregar com includes para o mapeamento correto
            var createdGenero = await _context.Generos
                .Include(g => g.Shows)
                .FirstAsync(g => g.Id == genero.Id);

            return _mapper.Map<GeneroDTO>(createdGenero);
        }

        public async Task<GeneroDTO?> UpdateGeneroAsync(int id, GeneroCreateUpdateDTO generoDto)
        {
            var genero = await _context.Generos
                .Include(g => g.Shows)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genero == null)
                return null;

            // Mapear as propriedades do DTO para a entidade existente
            _mapper.Map(generoDto, genero);
            await _context.SaveChangesAsync();

            return _mapper.Map<GeneroDTO>(genero);
        }

        public async Task<bool> DeleteGeneroAsync(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero == null)
                return false;

            _context.Generos.Remove(genero);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GeneroPopularDTO>> GetGenerosPopularesAsync(int take = 10)
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .OrderByDescending(g => g.Shows.Count())
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GeneroPopularDTO>>(generos);
        }

        public async Task<IEnumerable<GeneroDTO>> SearchGenerosByNameAsync(string nome)
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .Where(g => g.GeneroNome.Contains(nome))
                .ToListAsync();

            return _mapper.Map<IEnumerable<GeneroDTO>>(generos);
        }

        public async Task<bool> GeneroExistsAsync(int id)
        {
            return await _context.Generos.AnyAsync(g => g.Id == id);
        }

        public async Task<bool> GeneroExistsByNameAsync(string nome)
        {
            return await _context.Generos.AnyAsync(g => g.GeneroNome.ToLower() == nome.ToLower());
        }

        public async Task<int> GetTotalGenerosAsync()
        {
            return await _context.Generos.CountAsync();
        }

        public async Task<IEnumerable<GeneroDTO>> GetGenerosByShowCountRangeAsync(int minShows, int maxShows)
        {
            var generos = await _context.Generos
                .Include(g => g.Shows)
                .Where(g => g.Shows.Count() >= minShows && g.Shows.Count() <= maxShows)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GeneroDTO>>(generos);
        }
    }
}