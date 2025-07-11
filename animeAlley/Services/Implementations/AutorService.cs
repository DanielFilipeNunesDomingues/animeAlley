using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.AutoresDTO;
using animeAlley.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services.Implementations
{
    public class AutorService : IAutorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AutorDTO>> GetAllAutoresAsync()
        {
            var autores = await _context.Autores
                .Include(a => a.ShowsCriados)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AutorDTO>>(autores);
        }

        public async Task<AutorDTO?> GetAutorByIdAsync(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(a => a.Id == id);

            return autor != null ? _mapper.Map<AutorDTO>(autor) : null;
        }

        public async Task<AutorDTO> CreateAutorAsync(AutorCreateUpdateDTO autorDto)
        {
            var autor = _mapper.Map<Autor>(autorDto);

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return _mapper.Map<AutorDTO>(autor);
        }

        public async Task<AutorDTO?> UpdateAutorAsync(int id, AutorCreateUpdateDTO autorDto)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
                return null;

            _mapper.Map(autorDto, autor);
            await _context.SaveChangesAsync();

            return _mapper.Map<AutorDTO>(autor);
        }

        public async Task<bool> DeleteAutorAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
                return false;

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AutorPopularDTO>> GetAutoresPopularesAsync(int take = 10)
        {
            var autoresPopulares = await _context.Autores
                .Include(a => a.ShowsCriados)
                .OrderByDescending(a => a.ShowsCriados.Count())
                .ThenByDescending(a => a.ShowsCriados.Any() ? a.ShowsCriados.Average(s => s.Nota) : 0)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AutorPopularDTO>>(autoresPopulares);
        }

        public async Task<IEnumerable<AutorDTO>> SearchAutoresByNameAsync(string nome)
        {
            var autores = await _context.Autores
                .Include(a => a.ShowsCriados)
                .Where(a => a.Nome.Contains(nome))
                .ToListAsync();

            return _mapper.Map<IEnumerable<AutorDTO>>(autores);
        }

        public async Task<IEnumerable<AutorDTO>> GetAutoresByIdadeRangeAsync(int idadeMin, int idadeMax)
        {
            var autores = await _context.Autores
                .Include(a => a.ShowsCriados)
                .Where(a => a.Idade >= idadeMin && a.Idade <= idadeMax)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AutorDTO>>(autores);
        }

        public async Task<IEnumerable<AutorDTO>> GetAutoresBySexoAsync(string sexo)
        {
            var autores = await _context.Autores
                .Include(a => a.ShowsCriados)
                .Where(a => a.AutorSexualidade.ToString() == sexo)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AutorDTO>>(autores);
        }

        public async Task<bool> AutorExistsAsync(int id)
        {
            return await _context.Autores.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> AutorExistsByNameAsync(string nome)
        {
            return await _context.Autores.AnyAsync(a => a.Nome == nome);
        }

        public async Task<int> GetTotalAutoresAsync()
        {
            return await _context.Autores.CountAsync();
        }
    }
}