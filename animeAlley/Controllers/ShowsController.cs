using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;

namespace animeAlley.Controllers
{
    public class ShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// objeto que contém todas as características do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShowsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Shows.Include(s => s.Autor).Include(s => s.Studio);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // GET: Shows/Create
        public IActionResult Create()
        {
            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome");
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome");
            return View();
        }

        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sinopse,Tipo,Status,NotaAux,Ano,Imagem,Banner,Trailer,Views,Fonte,StudioFK,AutorFK")] Show show, IFormFile showFoto, IFormFile showBanner)
        {
            bool hasError = false;
            string imagePath = string.Empty;
            string bannerPath = string.Empty;

            if (show.AutorFK <= 0)
            {
                // Erro. Não foi escolhida uma categoria
                hasError = true;
                // crio msg de erro
                ModelState.AddModelError("", "Tem de escolher um Autor");
            }

            if (show.StudioFK <= 0)
            {
                // Erro. Não foi escolhida uma categoria
                hasError = true;
                // crio msg de erro
                ModelState.AddModelError("", "Tem de escolher um Studio");
            }

            if(showFoto == null || showBanner == null)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de submeter uma Fotografia do Show");
            } 
            else
            {
                if ((showFoto.ContentType != "image/jpeg" && showFoto.ContentType != "image/png") || (showBanner.ContentType != "image/jpeg" && showBanner.ContentType != "image/png"))
                {
                    // não há imagem
                    hasError = true;
                    // construo a msg de erro
                    ModelState.AddModelError("", "Tem de submeter uma Fotografia");
                } 
                else
                {
                    // há imagem,
                    // vamos processá-la
                    // *******************************
                    // Novo nome para o ficheiro
                    Guid g = Guid.NewGuid();
                    imagePath = g.ToString();
                    bannerPath = g.ToString();

                    string extensaoImagem = Path.GetExtension(showFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;

                    string extensaoBanner = Path.GetExtension(showBanner.FileName).ToLowerInvariant();
                    bannerPath += extensaoBanner;


                    // guardar este nome na BD
                    show.Imagem = imagePath;
                    show.Banner = bannerPath;
                }
            }

            ModelState.Remove("Imagem");
            ModelState.Remove("Banner");


            if (ModelState.IsValid && !hasError)
            {

                show.Nota = Convert.ToDecimal(show.NotaAux.Replace(".", ","), new CultureInfo("pt-PT"));

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
                // gerar o caminho completo para a imagem
                imagePath = Path.Combine(localizacaoImagem, imagePath);
                bannerPath = Path.Combine(localizacaoBanner, bannerPath);

                // agora, temos condições para guardar a imagem
                using var streamImage = new FileStream(
                   imagePath, FileMode.Create
                   );
                await showFoto.CopyToAsync(streamImage);

                using var streamBanner = new FileStream(
                   bannerPath, FileMode.Create
                   );
                await showBanner.CopyToAsync(streamBanner);
                // **********************************************

                return RedirectToAction(nameof(Index));
            }
            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome", show.AutorFK);
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome", show.StudioFK);
            return View(show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome", show.AutorFK);
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome", show.StudioFK);
            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sinopse,Tipo,Status,Nota,Ano,Imagem,Trailer,Views,Fonte,StudioFK,AutorFK")] Show show)
        {
            if (id != show.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome", show.AutorFK);
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome", show.StudioFK);
            return View(show);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Autor)
                .Include(s => s.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show != null)
            {
                _context.Shows.Remove(show);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}
