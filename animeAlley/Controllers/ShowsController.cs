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
        public async Task<IActionResult> Create([Bind("Id,Nome,Sinopse,Tipo,Status,NotaAux,Ano,Imagem,Trailer,Views,Fonte,StudioFK,AutorFK")] Show show, IFormFile showFoto)
        {
            bool hasError = false;
            string imagePath = string.Empty;

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

            if(showFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de submeter uma Fotografia do Show");
            } 
            else
            {
                if (showFoto.ContentType != "image/jpeg" && showFoto.ContentType != "image/png")
                {
                    // !(A==b || A==c) <=> (A!=b && A!=c)

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
                    string extensao = Path.GetExtension(showFoto.FileName).ToLowerInvariant();
                    imagePath += extensao;

                    // guardar este nome na BD
                    show.Imagem = imagePath;
                }
            }

            ModelState.Remove("Imagem");


            if (ModelState.IsValid && !hasError)
            {

                show.Nota = Convert.ToDecimal(show.NotaAux.Replace(".", ","), new CultureInfo("pt-PT"));

                _context.Add(show);
                await _context.SaveChangesAsync();

                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                localizacaoImagem = Path.Combine(localizacaoImagem, "images/showCover");
                if (!Directory.Exists(localizacaoImagem))
                {
                    Directory.CreateDirectory(localizacaoImagem);
                }
                // gerar o caminho completo para a imagem
                imagePath = Path.Combine(localizacaoImagem, imagePath);
                // agora, temos condições para guardar a imagem
                using var stream = new FileStream(
                   imagePath, FileMode.Create
                   );
                await showFoto.CopyToAsync(stream);
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
