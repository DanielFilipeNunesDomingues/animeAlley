using animeAlley.Data;
using animeAlley.Models;
using animeAlley.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace animeAlley.Controllers
{
    public class ShowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ShowsHub> _hubContext;

        public ShowsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IHubContext<ShowsHub> hubContext)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        // GET: Shows
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var shows = from s in _context.Shows
                        select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                shows = shows.Where(s =>
                    s.Nome.ToLower().Contains(lowerSearch) ||
                    s.Sinopse.ToLower().Contains(lowerSearch) ||
                    s.Studio.Nome.ToLower().Contains(lowerSearch));
            }

            return View(await shows.Include(s => s.Studio).ToListAsync());
        }

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Autor)
                .Include(s => s.Studio)
                .Include(s => s.Personagens)
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            // Incrementar views automaticamente ao acessar os detalhes
            await IncrementShowViews(id.Value);

            return View(show);
        }

        /// <summary>
        /// Método para incrementar as views de um show via SignalR
        /// </summary>
        private async Task IncrementShowViews(int showId)
        {
            try
            {
                var show = await _context.Shows.FindAsync(showId);
                if (show != null)
                {
                    show.Views++;
                    show.DataAtualizacao = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notificar via SignalR
                    await _hubContext.Clients.Group($"Show_{showId}").SendAsync("ViewUpdated", showId, show.Views);
                    await _hubContext.Clients.Group("ShowsIndex").SendAsync("ShowViewUpdated", showId, show.Views);
                }
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao incrementar views do show {showId}: {ex.Message}");
            }
        }

        /// <summary>
        /// API endpoint para incrementar views via AJAX (alternativa)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> IncrementViews(int showId)
        {
            try
            {
                await IncrementShowViews(showId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Shows/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome");
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome");
            ViewData["Generos"] = new MultiSelectList(_context.Generos, "Id", "GeneroNome");
            return View();
        }

        // POST: Shows/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sinopse,Tipo,Status,NotaAux,Ano,Imagem,Banner,Trailer,Views,Fonte,StudioFK,AutorFK")] Show show, IFormFile showFoto, IFormFile showBanner, int[] selectedGeneros)
        {
            bool hasError = false;
            string imagePath = string.Empty;
            string bannerPath = string.Empty;

            if (show.AutorFK <= 0)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de escolher um Autor");
            }

            if (show.StudioFK <= 0)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de escolher um Studio");
            }

            if (selectedGeneros == null || selectedGeneros.Length == 0)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de escolher pelo menos um Género");
            }

            if (showFoto == null || showBanner == null)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de submeter uma Fotografia do Show");
            }
            else
            {
                if ((showFoto.ContentType != "image/jpeg" && showFoto.ContentType != "image/png") || (showBanner.ContentType != "image/jpeg" && showBanner.ContentType != "image/png"))
                {
                    hasError = true;
                    ModelState.AddModelError("", "Tem de submeter uma Fotografia");
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    imagePath = g.ToString();
                    bannerPath = g.ToString();

                    string extensaoImagem = Path.GetExtension(showFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;

                    string extensaoBanner = Path.GetExtension(showBanner.FileName).ToLowerInvariant();
                    bannerPath += extensaoBanner;

                    show.Imagem = imagePath;
                    show.Banner = bannerPath;
                }
            }

            ModelState.Remove("Imagem");
            ModelState.Remove("Banner");
            ModelState.Remove("GenerosShows");

            if (ModelState.IsValid && !hasError)
            {
                show.Nota = Convert.ToDecimal(show.NotaAux.Replace(".", ","), new CultureInfo("pt-PT"));

                if (selectedGeneros != null && selectedGeneros.Length > 0)
                {
                    var generos = await _context.Generos
                        .Where(g => selectedGeneros.Contains(g.Id))
                        .ToListAsync();

                    show.GenerosShows = generos;
                }

                _context.Add(show);
                await _context.SaveChangesAsync();

                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                string localizacaoBanner = _webHostEnvironment.WebRootPath;
                localizacaoImagem = Path.Combine(localizacaoImagem, "images/showCover");
                localizacaoBanner = Path.Combine(localizacaoBanner, "images/showBanner");

                if (!Directory.Exists(localizacaoImagem))
                {
                    Directory.CreateDirectory(localizacaoImagem);
                }
                if (!Directory.Exists(localizacaoBanner))
                {
                    Directory.CreateDirectory(localizacaoBanner);
                }

                imagePath = Path.Combine(localizacaoImagem, imagePath);
                bannerPath = Path.Combine(localizacaoBanner, bannerPath);

                using var streamImage = new FileStream(imagePath, FileMode.Create);
                await showFoto.CopyToAsync(streamImage);

                using var streamBanner = new FileStream(bannerPath, FileMode.Create);
                await showBanner.CopyToAsync(streamBanner);

                return RedirectToAction(nameof(Index));
            }

            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome", show.AutorFK);
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome", show.StudioFK);
            ViewData["Generos"] = new MultiSelectList(_context.Generos, "Id", "GeneroNome", selectedGeneros);
            return View(show);
        }

        // Manter os demais métodos (Edit, Delete, etc.) inalterados...
        // [Os outros métodos permanecem os mesmos do código original]

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}