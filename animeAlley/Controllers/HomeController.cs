using animeAlley.Data;
using animeAlley.Models;
using animeAlley.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animeAlley.Controllers
{
    /// <summary>
    /// Controller principal responsável pela página inicial, pesquisa e tratamento de erros
    /// </summary>
    public class HomeController : Controller
    {
        #region Propriedades e Construtor

        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do HomeController
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="logger">Serviço de logging</param>
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        #region Ações Principais

        /// <summary>
        /// Página inicial da aplicação
        /// Exibe shows em destaque, recentes, personagens, autores e studios
        /// </summary>
        /// <returns>View com dados da página inicial</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Buscar um show aleatório para o banner principal
                var randomShow = await GetRandomShowWithBanner();

                var homeViewModel = new BrowseViewModel
                {
                    // Show aleatório para o banner da página inicial
                    RandomBannerShow = randomShow,

                    // Shows mais bem avaliados (top 6 por nota)
                    TopShows = await _context.Shows
                        .Include(s => s.Studio)
                        .Include(s => s.Autor)
                        .Include(s => s.GenerosShows)
                        .OrderByDescending(s => s.Nota)
                        .Take(6)
                        .ToListAsync(),

                    // Shows mais recentes (últimos 6 criados)
                    RecentShows = await _context.Shows
                        .Include(s => s.Studio)
                        .Include(s => s.Autor)
                        .Include(s => s.GenerosShows)
                        .OrderByDescending(s => s.DataCriacao)
                        .Take(6)
                        .ToListAsync(),

                    // Personagens mais recentes (últimos 6 por ID)
                    RecentPersonagens = await _context.Personagens
                        .Include(p => p.Shows)
                        .OrderByDescending(p => p.Id)
                        .Take(6)
                        .ToListAsync(),

                    // Autores mais recentes (últimos 6 por ID)
                    RecentAutores = await _context.Autores
                        .Include(a => a.ShowsCriados)
                        .OrderByDescending(a => a.Id)
                        .Take(6)
                        .ToListAsync(),

                    // Studios mais recentes (últimos 6 por ID)
                    RecentStudios = await _context.Studios
                        .Include(s => s.ShowsDesenvolvidos)
                        .OrderByDescending(s => s.Id)
                        .Take(6)
                        .ToListAsync()
                };

                return View(homeViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página inicial");
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Funcionalidade de pesquisa global na aplicação
        /// Permite buscar em shows, personagens, autores e studios
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="filterType">Tipo de filtro (all, shows, personagens, autores, studios)</param>
        /// <returns>View com resultados da pesquisa</returns>
        [HttpGet]
        public async Task<IActionResult> Search(string searchString, string filterType = "all")
        {
            try
            {
                // Buscar um show aleatório para o banner também na pesquisa
                var randomShow = await GetRandomShowWithBanner();

                // Se não há termo de pesquisa, retornar resultados vazios
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    var emptyResults = new SearchResultsViewModel
                    {
                        RandomBannerShow = randomShow,
                        SearchTerm = string.Empty,
                        FilterType = filterType
                    };
                    return View("SearchResults", emptyResults);
                }

                var searchResults = new SearchResultsViewModel
                {
                    SearchTerm = searchString,
                    FilterType = filterType,
                    RandomBannerShow = randomShow
                };

                var lowerSearch = searchString.ToLower();

                // Pesquisar em Shows (nome, sinopse, studio, autor)
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

                // Pesquisar em Personagens (nome e sinopse)
                if (filterType == "all" || filterType == "personagens")
                {
                    searchResults.Personagens = await _context.Personagens
                        .Include(p => p.Shows)
                        .Where(p => p.Nome.ToLower().Contains(lowerSearch) ||
                                   p.Sinopse.ToLower().Contains(lowerSearch))
                        .OrderBy(p => p.Nome)
                        .ToListAsync();
                }

                // Pesquisar em Autores (nome e sobre)
                if (filterType == "all" || filterType == "autores")
                {
                    searchResults.Autores = await _context.Autores
                        .Include(a => a.ShowsCriados)
                        .Where(a => a.Nome.ToLower().Contains(lowerSearch) ||
                                   a.Sobre.ToLower().Contains(lowerSearch))
                        .OrderBy(a => a.Nome)
                        .ToListAsync();
                }

                // Pesquisar em Studios (nome e sobre)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante pesquisa: {SearchTerm}", searchString);
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Exibe seções específicas com mais resultados (expandir seções da página inicial)
        /// </summary>
        /// <param name="section">Nome da seção a ser exibida</param>
        /// <returns>View com dados da seção específica</returns>
        public async Task<IActionResult> ShowSection(string section)
        {
            try
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
                        _logger.LogWarning("Seção desconhecida solicitada: {Section}", section);
                        return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar seção: {Section}", section);
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Página de privacidade da aplicação
        /// </summary>
        /// <returns>View da página de privacidade</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Método privado para buscar um show aleatório que possui banner
        /// Usado para exibir banners dinâmicos na página inicial e pesquisa
        /// </summary>
        /// <returns>Show aleatório com banner ou null se não encontrar</returns>
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
                    // Selecionar um show aleatório da lista
                    var random = new Random();
                    var randomIndex = random.Next(showsWithBanner.Count);
                    return showsWithBanner[randomIndex];
                }

                // Se não houver shows com banner, retornar qualquer show disponível
                var anyShow = await _context.Shows.FirstOrDefaultAsync();
                return anyShow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar show aleatório com banner");
                // Em caso de erro, retornar null para não quebrar a aplicação
                return null;
            }
        }

        #endregion

        #region Tratamento de Erros

        /// <summary>
        /// Trata erros gerais da aplicação
        /// Exibe uma página de erro genérica com informações mínimas
        /// </summary>
        /// <returns>View de erro genérico</returns>
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Erro geral ocorreu. RequestId: {RequestId}", requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = 500,
                ErrorTitle = "Erro Interno do Servidor",
                ErrorMessage = "Ocorreu um erro inesperado. Nossa equipe foi notificada.",
                ErrorDescription = "Pedimos desculpas pela inconveniência. Tente novamente em alguns minutos.",
                ShowTechnicalDetails = false
            };

            return View("Error", model);
        }

        /// <summary>
        /// Trata erros específicos baseados no código de status HTTP
        /// Fornece mensagens personalizadas para cada tipo de erro
        /// </summary>
        /// <param name="statusCode">Código de status HTTP do erro</param>
        /// <returns>View de erro personalizada baseada no status code</returns>
        [HttpGet]
        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var originalPath = HttpContext.Request.Path.Value;

            _logger.LogWarning("Erro de código de status: {StatusCode} para o caminho: {OriginalPath}. RequestId: {RequestId}",
                statusCode, originalPath, requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = statusCode,
                OriginalPath = originalPath,
                ShowTechnicalDetails = false
            };

            // Personalizar mensagem baseada no código de status
            switch (statusCode)
            {
                case 400:
                    model.ErrorTitle = "Solicitação Inválida";
                    model.ErrorMessage = "A solicitação enviada não é válida.";
                    model.ErrorDescription = "Verifique os dados enviados e tente novamente.";
                    break;

                case 401:
                    model.ErrorTitle = "Não Autorizado";
                    model.ErrorMessage = "Você precisa fazer login para acessar esta página.";
                    model.ErrorDescription = "Faça login com suas credenciais para continuar.";
                    break;

                case 403:
                    model.ErrorTitle = "Acesso Negado";
                    model.ErrorMessage = "Você não tem permissão para acessar este recurso.";
                    model.ErrorDescription = "Esta página requer privilégios especiais que você não possui.";
                    break;

                case 404:
                    model.ErrorTitle = "Página Não Encontrada";
                    model.ErrorMessage = "A página que você está procurando não foi encontrada.";
                    model.ErrorDescription = "A página pode ter sido movida, renomeada ou não existe mais.";
                    break;

                case 408:
                    model.ErrorTitle = "Tempo Limite Excedido";
                    model.ErrorMessage = "A solicitação demorou muito tempo para ser processada.";
                    model.ErrorDescription = "Tente novamente em alguns momentos.";
                    break;

                case 429:
                    model.ErrorTitle = "Muitas Solicitações";
                    model.ErrorMessage = "Você fez muitas solicitações em pouco tempo.";
                    model.ErrorDescription = "Aguarde alguns minutos antes de tentar novamente.";
                    break;

                case 500:
                    model.ErrorTitle = "Erro Interno do Servidor";
                    model.ErrorMessage = "Ocorreu um erro interno no servidor.";
                    model.ErrorDescription = "Nossa equipe foi notificada e está trabalhando para resolver o problema.";
                    break;

                case 502:
                    model.ErrorTitle = "Gateway Inválido";
                    model.ErrorMessage = "O servidor está temporariamente indisponível.";
                    model.ErrorDescription = "Tente novamente em alguns minutos.";
                    break;

                case 503:
                    model.ErrorTitle = "Serviço Indisponível";
                    model.ErrorMessage = "O serviço está temporariamente indisponível.";
                    model.ErrorDescription = "Estamos fazendo manutenção. Tente novamente em breve.";
                    break;

                default:
                    model.ErrorTitle = $"Erro {statusCode}";
                    model.ErrorMessage = "Ocorreu um erro inesperado.";
                    model.ErrorDescription = "Tente novamente ou entre em contato com o suporte.";
                    break;
            }

            // Definir o código de status da resposta
            Response.StatusCode = statusCode;

            return View("Error", model);
        }

        /// <summary>
        /// Página de erro personalizada para ambiente de desenvolvimento
        /// Exibe informações técnicas detalhadas para facilitar debug
        /// </summary>
        /// <param name="message">Mensagem de erro opcional</param>
        /// <param name="stackTrace">Stack trace da exceção opcional</param>
        /// <returns>View de erro de desenvolvimento com detalhes técnicos</returns>
        [HttpGet]
        [Route("Home/ErrorDev")]
        public IActionResult ErrorDev(string? message = null, string? stackTrace = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Erro de desenvolvimento detectado. RequestId: {RequestId}, Message: {Message}",
                requestId, message);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = 500,
                ErrorTitle = "Erro de Desenvolvimento",
                ErrorMessage = message ?? "Erro detectado durante o desenvolvimento",
                ErrorDescription = "Informações técnicas detalhadas disponíveis abaixo",
                ShowTechnicalDetails = true,
                ExceptionMessage = stackTrace,
                OriginalPath = HttpContext.Request.Path.Value
            };

            return View("Error", model);
        }

        #endregion
    }
}