using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;

namespace animeAlley.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Página principal de administração com estatísticas
        /// </summary>
        public async Task<IActionResult> Administrar()
        {
            var viewModel = new AdminDashboardViewModel();

            try
            {
                viewModel.TotalShows = await _context.Shows.CountAsync();
                viewModel.TotalAutores = await _context.Autores.CountAsync();
                viewModel.TotalStudios = await _context.Studios.CountAsync();
                viewModel.TotalPersonagens = await _context.Personagens.CountAsync();
                viewModel.TotalGeneros = await _context.Generos.CountAsync();
                viewModel.TotalUtilizadores = await _context.Utilizadores.CountAsync();

                // Shows mais visualizados (top 5)
                var showsMaisVisualizados = await _context.Shows
                    .OrderByDescending(s => s.Views)
                    .Take(5)
                    .Select(s => new ShowInfoDto { Nome = s.Nome, Id = s.Id , Views = s.Views })
                    .ToListAsync();
                viewModel.ShowsMaisVisualizados = showsMaisVisualizados;

                // Shows mais recentes (top 5)
                var showsRecentes = await _context.Shows
                    .OrderByDescending(s => s.DataCriacao)
                    .Take(5)
                    .Select(s => new ShowRecenteDto { Nome = s.Nome, Id = s.Id, DataCriacao = s.DataCriacao })
                    .ToListAsync();
                viewModel.ShowsRecentes = showsRecentes;
            }
            catch (Exception)
            {
                // Em caso de erro, retorna valores padrão
                viewModel.TotalShows = 0;
                viewModel.TotalAutores = 0;
                viewModel.TotalStudios = 0;
                viewModel.TotalPersonagens = 0;
                viewModel.TotalGeneros = 0;
                viewModel.TotalUtilizadores = 0;
                viewModel.ShowsMaisVisualizados = new List<ShowInfoDto>();
                viewModel.ShowsRecentes = new List<ShowRecenteDto>();
            }

            return View(viewModel);
        }
    }

    /// <summary>
    /// ViewModel para a dashboard de administração
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalShows { get; set; }
        public int TotalAutores { get; set; }
        public int TotalStudios { get; set; }
        public int TotalPersonagens { get; set; }
        public int TotalGeneros { get; set; }
        public int TotalUtilizadores { get; set; }

        public List<ShowInfoDto> ShowsMaisVisualizados { get; set; } = new List<ShowInfoDto>();
        public List<ShowRecenteDto> ShowsRecentes { get; set; } = new List<ShowRecenteDto>();
    }

    /// <summary>
    /// DTO para informações básicas do show
    /// </summary>
    public class ShowInfoDto
    {
        public string Nome { get; set; } = string.Empty;
        public int Id { get; set; }
        public int Views { get; set; }
    }

    /// <summary>
    /// DTO para shows recentes
    /// </summary>
    public class ShowRecenteDto
    {
        public string Nome { get; set; } = string.Empty;
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}