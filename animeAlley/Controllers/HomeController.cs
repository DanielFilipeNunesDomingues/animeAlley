using animeAlley.Data;
using animeAlley.Models;
using animeAlley.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animeAlley.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Buscar um show aleatório para o banner
            var randomShow = await GetRandomShowWithBanner();

            var homeViewModel = new BrowseViewModel
            {
                // Show aleatório para o banner
                RandomBannerShow = randomShow,

                // Shows mais bem avaliados (top 6)
                TopShows = await _context.Shows
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.GenerosShows)
                    .OrderByDescending(s => s.Nota)
                    .Take(6)
                    .ToListAsync(),

                // Shows mais recentes (ultimos 6)
                RecentShows = await _context.Shows
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.GenerosShows)
                    .OrderByDescending(s => s.DataCriacao)
                    .Take(6)
                    .ToListAsync(),

                // Personagens mais recentes (ultimos 6)
                RecentPersonagens = await _context.Personagens
                    .Include(p => p.Shows)
                    .OrderByDescending(p => p.Id)
                    .Take(6)
                    .ToListAsync(),

                // Autores mais recentes (ultimos 6)
                RecentAutores = await _context.Autores
                    .Include(a => a.ShowsCriados)
                    .OrderByDescending(a => a.Id)
                    .Take(6)
                    .ToListAsync(),

                // Studios mais recentes (ultimos 6)
                RecentStudios = await _context.Studios
                    .Include(s => s.ShowsDesenvolvidos)
                    .OrderByDescending(s => s.Id)
                    .Take(6)
                    .ToListAsync()
            };

            return View(homeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString, string filterType = "all")
        {
            // Buscar um show aleatório para o banner também na pesquisa
            var randomShow = await GetRandomShowWithBanner();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                var emptyResults = new SearchResultsViewModel();
                emptyResults.RandomBannerShow = randomShow;
                return View("SearchResults", emptyResults);
            }

            var searchResults = new SearchResultsViewModel
            {
                SearchTerm = searchString,
                FilterType = filterType,
                RandomBannerShow = randomShow
            };

            var lowerSearch = searchString.ToLower();

            // Pesquisar em Shows
            if (filterType == "all" || filterType == "shows")
            {
                searchResults.Shows = await _context.Shows
                    .Include(s => s.Studio)
                    .Include(s => s.Autor)
                    .Include(s => s.GenerosShows)
                    .Where(s => s.Nome.ToLower().Contains(lowerSearch) ||
                               s.Sinopse.ToLower().Contains(lowerSearch) ||
                               s.Studio.Nome.ToLower().Contains(lowerSearch) ||
                               s.Autor.Nome.ToLower().Contains(lowerSearch))
                    .OrderByDescending(s => s.Nota)
                    .ToListAsync();
            }

            // Pesquisar em Personagens
            if (filterType == "all" || filterType == "personagens")
            {
                searchResults.Personagens = await _context.Personagens
                    .Include(p => p.Shows)
                    .Where(p => p.Nome.ToLower().Contains(lowerSearch) ||
                               p.Sinopse.ToLower().Contains(lowerSearch))
                    .OrderBy(p => p.Nome)
                    .ToListAsync();
            }

            // Pesquisar em Autores
            if (filterType == "all" || filterType == "autores")
            {
                searchResults.Autores = await _context.Autores
                    .Include(a => a.ShowsCriados)
                    .Where(a => a.Nome.ToLower().Contains(lowerSearch) ||
                               a.Sobre.ToLower().Contains(lowerSearch))
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }

            // Pesquisar em Studios
            if (filterType == "all" || filterType == "studios")
            {
                searchResults.Studios = await _context.Studios
                    .Include(s => s.ShowsDesenvolvidos)
                    .Where(s => s.Nome.ToLower().Contains(lowerSearch) ||
                               (s.Sobre != null && s.Sobre.ToLower().Contains(lowerSearch)))
                    .OrderBy(s => s.Nome)
                    .ToListAsync();
            }

            return View("SearchResults", searchResults);
        }

        // Método privado para buscar um show aleatório com banner
        private async Task<Show?> GetRandomShowWithBanner()
        {
            try
            {
                // Buscar shows que têm banner (assumindo que Banner não é null/empty)
                var showsWithBanner = await _context.Shows
                    .Where(s => !string.IsNullOrEmpty(s.Banner))
                    .ToListAsync();

                if (showsWithBanner.Any())
                {
                    // Selecionar um aleatório
                    var random = new Random();
                    var randomIndex = random.Next(showsWithBanner.Count);
                    return showsWithBanner[randomIndex];
                }

                // Se não houver shows com banner, retornar qualquer show
                var anyShow = await _context.Shows.FirstOrDefaultAsync();
                return anyShow;
            }
            catch
            {
                // Em caso de erro, retornar null
                return null;
            }
        }

        // Acao para exibir secao especifica com mais resultados
        public async Task<IActionResult> ShowSection(string section)
        {
            switch (section.ToLower())
            {
                case "topshows":
                    var topShows = await _context.Shows
                        .Include(s => s.Studio)
                        .Include(s => s.Autor)
                        .Include(s => s.GenerosShows)
                        .OrderByDescending(s => s.Nota)
                        .Take(24)
                        .ToListAsync();
                    ViewBag.SectionTitle = "Melhores Shows";
                    return View("ShowsSection", topShows);

                case "recentshows":
                    var recentShows = await _context.Shows
                        .Include(s => s.Studio)
                        .Include(s => s.Autor)
                        .Include(s => s.GenerosShows)
                        .OrderByDescending(s => s.DataCriacao)
                        .Take(24)
                        .ToListAsync();
                    ViewBag.SectionTitle = "Shows Recentes";
                    return View("ShowsSection", recentShows);

                case "personagens":
                    var personagens = await _context.Personagens
                        .Include(p => p.Shows)
                        .OrderByDescending(p => p.Id)
                        .Take(24)
                        .ToListAsync();
                    ViewBag.SectionTitle = "Personagens Recentes";
                    return View("PersonagensSection", personagens);

                case "autores":
                    var autores = await _context.Autores
                        .Include(a => a.ShowsCriados)
                        .OrderByDescending(a => a.Id)
                        .Take(24)
                        .ToListAsync();
                    ViewBag.SectionTitle = "Autores Recentes";
                    return View("AutoresSection", autores);

                case "studios":
                    var studios = await _context.Studios
                        .Include(s => s.ShowsDesenvolvidos)
                        .OrderByDescending(s => s.Id)
                        .Take(24)
                        .ToListAsync();
                    ViewBag.SectionTitle = "Studios Recentes";
                    return View("StudiosSection", studios);

                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Método existente para erros gerais
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Novo método para tratar códigos de status específicos
        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode
            };

            // Personalizar mensagem baseada no código de status
            switch (statusCode)
            {
                case 404:
                    model.ErrorMessage = "A página que você está procurando não foi encontrada.";
                    model.ErrorTitle = "Página não encontrada (404)";
                    break;
                case 403:
                    model.ErrorMessage = "Você não tem permissão para acessar este recurso.";
                    model.ErrorTitle = "Acesso negado (403)";
                    break;
                case 500:
                    model.ErrorMessage = "Ocorreu um erro interno no servidor.";
                    model.ErrorTitle = "Erro interno do servidor (500)";
                    break;
                default:
                    model.ErrorMessage = "Ocorreu um erro inesperado.";
                    model.ErrorTitle = $"Erro ({statusCode})";
                    break;
            }

            return View("Error", model);
        }
    }
}