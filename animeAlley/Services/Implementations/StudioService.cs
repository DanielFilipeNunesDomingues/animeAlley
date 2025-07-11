using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Models.ViewModels.StudiosDTO;
using animeAlley.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Services
{
    public class StudioService : IStudioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StudioService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Operações CRUD básicas
        public async Task<IEnumerable<StudioDTO>> GetAllStudiosAsync()
        {
            var studios = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudioDTO>>(studios);
        }

        public async Task<StudioDTO?> GetStudioByIdAsync(int id)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            return studio != null ? _mapper.Map<StudioDTO>(studio) : null;
        }

        public async Task<StudioDTO> CreateStudioAsync(StudioCreateUpdateDTO studioDto)
        {
            var studio = _mapper.Map<Studio>(studioDto);

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudioDTO>(studio);
        }

        public async Task<StudioDTO?> UpdateStudioAsync(int id, StudioCreateUpdateDTO studioDto)
        {
            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null)
                return null;

            _mapper.Map(studioDto, studio);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudioDTO>(studio);
        }

        public async Task<bool> DeleteStudioAsync(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
                return false;

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return true;
        }

        // Funcionalidades adicionais
        public async Task<IEnumerable<StudioPopularDTO>> GetStudiosPopularesAsync(int take = 10)
        {
            var studiosPopulares = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .OrderByDescending(s => s.ShowsDesenvolvidos.Count())
                .ThenByDescending(s => s.ShowsDesenvolvidos.Any() ? s.ShowsDesenvolvidos.Average(show => show.Nota) : 0)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudioPopularDTO>>(studiosPopulares);
        }

        public async Task<IEnumerable<StudioDTO>> SearchStudiosByNameAsync(string nome)
        {
            var studios = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .Where(s => s.Nome.Contains(nome))
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudioDTO>>(studios);
        }

        public async Task<IEnumerable<StudioDTO>> GetStudiosByStatusAsync(string status)
        {
            if (!Enum.TryParse<Estado>(status, out var estadoEnum))
                return new List<StudioDTO>();

            var studios = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .Where(s => s.Status == estadoEnum)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudioDTO>>(studios);
        }

        public async Task<IEnumerable<StudioDTO>> GetStudiosByFundadoRangeAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Studios
                .Include(s => s.ShowsDesenvolvidos)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(s => s.Fundado >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(s => s.Fundado <= dataFim.Value);

            var studios = await query.ToListAsync();

            return _mapper.Map<IEnumerable<StudioDTO>>(studios);
        }

        public async Task<IEnumerable<StudioDTO>> GetStudiosAtivosAsync()
        {
            return await GetStudiosByStatusAsync("Ativo");
        }

        public async Task<IEnumerable<StudioDTO>> GetStudiosInativosAsync()
        {
            return await GetStudiosByStatusAsync("Inativo");
        }

        public async Task<bool> StudioExistsAsync(int id)
        {
            return await _context.Studios.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> StudioExistsByNameAsync(string nome)
        {
            return await _context.Studios.AnyAsync(s => s.Nome == nome);
        }

        public async Task<object> GetTotalStudiosAsync()
        {
            var totalStudios = await _context.Studios.CountAsync();
            var totalStudioAtivos = await _context.Studios.CountAsync(s => s.Status == Estado.Ativo);
            var totalStudioInativos = await _context.Studios.CountAsync(s => s.Status == Estado.Inativo);

            return new
            {
                TotalStudios = totalStudios,
                TotalStudioAtivos = totalStudioAtivos,
                TotalStudioInativos = totalStudioInativos
            };
        }
    }
}