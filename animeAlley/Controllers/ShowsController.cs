using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            return View(show);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ModelState.Remove("GenerosShows");


            if (ModelState.IsValid && !hasError)
            {

                show.Nota = Convert.ToDecimal(show.NotaAux.Replace(".", ","), new CultureInfo("pt-PT"));

                // Adicionar os géneros selecionados
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
            ViewData["Generos"] = new MultiSelectList(_context.Generos, "Id", "GeneroNome", selectedGeneros);
            return View(show);
        }

        // GET: Shows/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            // Converter decimal para string para o campo NotaAux
            show.NotaAux = show.Nota.ToString("F2", new CultureInfo("pt-PT"));

            ViewData["AutorFK"] = new SelectList(_context.Autores, "Id", "Nome", show.AutorFK);
            ViewData["StudioFK"] = new SelectList(_context.Studios, "Id", "Nome", show.StudioFK);

            // Carregar géneros selecionados
            var selectedGeneros = show.GenerosShows.Select(g => g.Id).ToArray();
            ViewData["Generos"] = new MultiSelectList(_context.Generos, "Id", "GeneroNome", selectedGeneros);

            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sinopse,Tipo,Status,NotaAux,Ano,Imagem,Banner,Trailer,Views,Fonte,StudioFK,AutorFK")] Show show, IFormFile showFoto, IFormFile showBanner, int[] selectedGeneros)
        {
            if (id != show.Id)
            {
                return NotFound();
            }

            bool hasError = false;
            string newImagePath = string.Empty;
            string newBannerPath = string.Empty;
            string oldImagePath = string.Empty;
            string oldBannerPath = string.Empty;

            // Carregar o show existente para obter os caminhos das imagens atuais
            var existingShow = await _context.Shows
                .Include(s => s.GenerosShows)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShow == null)
            {
                return NotFound();
            }

            // Guardar os caminhos das imagens antigas
            oldImagePath = existingShow.Imagem;
            oldBannerPath = existingShow.Banner;

            // Manter as imagens existentes se não foram fornecidas novas
            show.Imagem = oldImagePath;
            show.Banner = oldBannerPath;

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

            // Processar nova imagem se fornecida
            if (showFoto != null)
            {
                if (showFoto.ContentType != "image/jpeg" && showFoto.ContentType != "image/png")
                {
                    hasError = true;
                    ModelState.AddModelError("", "A imagem deve ser em formato JPEG ou PNG");
                }
                else
                {
                    // Gerar novo nome para a imagem
                    Guid g = Guid.NewGuid();
                    newImagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(showFoto.FileName).ToLowerInvariant();
                    newImagePath += extensaoImagem;
                    show.Imagem = newImagePath;
                }
            }

            // Processar novo banner se fornecido
            if (showBanner != null)
            {
                if (showBanner.ContentType != "image/jpeg" && showBanner.ContentType != "image/png")
                {
                    hasError = true;
                    ModelState.AddModelError("", "O banner deve ser em formato JPEG ou PNG");
                }
                else
                {
                    // Gerar novo nome para o banner
                    Guid g = Guid.NewGuid();
                    newBannerPath = g.ToString();
                    string extensaoBanner = Path.GetExtension(showBanner.FileName).ToLowerInvariant();
                    newBannerPath += extensaoBanner;
                    show.Banner = newBannerPath;
                }
            }

            ModelState.Remove("GenerosShows");
            ModelState.Remove("Imagem");
            ModelState.Remove("Banner");
            ModelState.Remove("showFoto");
            ModelState.Remove("showBanner");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    // Converter NotaAux para Nota
                    show.Nota = Convert.ToDecimal(show.NotaAux.Replace(".", ","), new CultureInfo("pt-PT"));

                    // Carregar o show existente com os géneros para atualização
                    var showToUpdate = await _context.Shows
                        .Include(s => s.GenerosShows)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (showToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Atualizar propriedades básicas
                    showToUpdate.Nome = show.Nome;
                    showToUpdate.Sinopse = show.Sinopse;
                    showToUpdate.Tipo = show.Tipo;
                    showToUpdate.Status = show.Status;
                    showToUpdate.Nota = show.Nota;
                    showToUpdate.Ano = show.Ano;
                    showToUpdate.Trailer = show.Trailer;
                    showToUpdate.Views = show.Views;
                    showToUpdate.Fonte = show.Fonte;
                    showToUpdate.StudioFK = show.StudioFK;
                    showToUpdate.AutorFK = show.AutorFK;
                    showToUpdate.Imagem = show.Imagem;
                    showToUpdate.Banner = show.Banner;

                    // Limpar géneros existentes
                    showToUpdate.GenerosShows.Clear();

                    // Adicionar novos géneros
                    if (selectedGeneros != null && selectedGeneros.Length > 0)
                    {
                        var generos = await _context.Generos
                            .Where(g => selectedGeneros.Contains(g.Id))
                            .ToListAsync();

                        foreach (var genero in generos)
                        {
                            showToUpdate.GenerosShows.Add(genero);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Processar as imagens após salvar na BD
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images/showCover");
                    string localizacaoBanner = Path.Combine(_webHostEnvironment.WebRootPath, "images/showBanner");

                    // Garantir que os diretórios existem
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }
                    if (!Directory.Exists(localizacaoBanner))
                    {
                        Directory.CreateDirectory(localizacaoBanner);
                    }

                    // Salvar nova imagem e remover a antiga
                    if (showFoto != null && !string.IsNullOrEmpty(newImagePath))
                    {
                        // Salvar nova imagem
                        string fullNewImagePath = Path.Combine(localizacaoImagem, newImagePath);
                        using var streamImage = new FileStream(fullNewImagePath, FileMode.Create);
                        await showFoto.CopyToAsync(streamImage);

                        // Remover imagem antiga se existir e for diferente
                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                        {
                            string fullOldImagePath = Path.Combine(localizacaoImagem, oldImagePath);
                            if (System.IO.File.Exists(fullOldImagePath))
                            {
                                System.IO.File.Delete(fullOldImagePath);
                            }
                        }
                    }

                    // Salvar novo banner e remover o antigo
                    if (showBanner != null && !string.IsNullOrEmpty(newBannerPath))
                    {
                        // Salvar novo banner
                        string fullNewBannerPath = Path.Combine(localizacaoBanner, newBannerPath);
                        using var streamBanner = new FileStream(fullNewBannerPath, FileMode.Create);
                        await showBanner.CopyToAsync(streamBanner);

                        // Remover banner antigo se existir e for diferente
                        if (!string.IsNullOrEmpty(oldBannerPath) && oldBannerPath != newBannerPath)
                        {
                            string fullOldBannerPath = Path.Combine(localizacaoBanner, oldBannerPath);
                            if (System.IO.File.Exists(fullOldBannerPath))
                            {
                                System.IO.File.Delete(fullOldBannerPath);
                            }
                        }
                    }
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
            ViewData["Generos"] = new MultiSelectList(_context.Generos, "Id", "GeneroNome", selectedGeneros);

            return View(show);
        }

        // GET: Shows/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Autor)
                .Include(s => s.Studio)
                .Include(s => s.GenerosShows)
                .Include(s => s.Personagens)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenerosShows)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (show != null)
            {
                // Remover as relações com géneros antes de deletar o show
                show.GenerosShows.Clear();
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
