using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Hosting; // Needed for IWebHostEnvironment
using System.IO; // Needed for Path
using Microsoft.AspNetCore.Authorization; // Needed for Authorize attribute

namespace animeAlley.Controllers
{
    public class AutoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AutoresController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Autores
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var autores = from a in _context.Autores
                          select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                autores = autores.Where(a =>
                    a.Nome.ToLower().Contains(lowerSearch) ||
                    a.Sobre.ToLower().Contains(lowerSearch)
                );
            }

            return View(await autores.ToListAsync());
        }

        /// <summary>
        /// GET: Autores/Details/5
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Details view for the Author</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
                .Include(a => a.ShowsCriados) // Eager load the collection of shows created by this author
                .FirstOrDefaultAsync(m => m.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // GET: Autores/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // No MultiSelectList needed for Authors, as they don't have an associated list like Shows
            return View();
        }

        // POST: Autores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,DateNasc,Sobre,Foto,Idade,AutorSexualidade")] Autor autor, IFormFile autorFoto)
        {
            bool hasError = false;
            string imagePath = string.Empty;

            if (autorFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("autorFoto", "Precisa de submeter uma fotografia do Autor.");
            }
            else
            {
                // Check content type and file size
                if (autorFoto.ContentType != "image/jpeg" && autorFoto.ContentType != "image/png")
                {
                    hasError = true;
                    ModelState.AddModelError("autorFoto", "A fotografia deve ser em formato JPEG ou PNG.");
                }
                else if (autorFoto.Length > 2 * 1024 * 1024) // 2 MB limit
                {
                    hasError = true;
                    ModelState.AddModelError("autorFoto", "O tamanho do arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    // Process the image
                    Guid g = Guid.NewGuid();
                    imagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(autorFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;
                    autor.Foto = imagePath;
                }
            }

            // Remove "Foto" from ModelState to prevent validation issues with null Foto property
            // when IFormFile is handled separately.
            ModelState.Remove("Foto");
            // Also remove the IFormFile itself, as it's not directly part of the model validation
            ModelState.Remove("autorFoto");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    _context.Add(autor);
                    await _context.SaveChangesAsync();

                    // Save the image physically
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "autoresFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    string caminhoCompletoImagem = Path.Combine(localizacaoImagem, imagePath);
                    using var streamImage = new FileStream(caminhoCompletoImagem, FileMode.Create);
                    await autorFoto.CopyToAsync(streamImage);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o autor: " + ex.Message);
                }
            }
            // If we got this far, something failed, re-display form
            return View(autor);
        }

        // GET: Autores/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // POST: Autores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DateNasc,Sobre,Foto,Idade,AutorSexualidade")] Autor autor, IFormFile autorFoto)
        {
            if (id != autor.Id)
            {
                return NotFound();
            }

            bool fileError = false;
            string newImagePath = string.Empty;
            string oldImagePath = string.Empty;

            var existingAutor = await _context.Autores
                .AsNoTracking() // Use AsNoTracking to avoid tracking conflicts when attaching 'autor' later
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAutor == null)
            {
                return NotFound();
            }

            oldImagePath = existingAutor.Foto;
            autor.Foto = oldImagePath; // Keep the old photo if no new one is uploaded

            if (autorFoto != null)
            {
                if (autorFoto.ContentType != "image/jpeg" && autorFoto.ContentType != "image/png")
                {
                    fileError = true;
                    ModelState.AddModelError("autorFoto", "A fotografia deve ser em formato JPEG ou PNG.");
                }
                else if (autorFoto.Length > 2 * 1024 * 1024) // 2 MB limit
                {
                    fileError = true;
                    ModelState.AddModelError("autorFoto", "O arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    newImagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(autorFoto.FileName).ToLowerInvariant();
                    newImagePath += extensaoImagem;
                    autor.Foto = newImagePath; // Assign new image name to model
                }
            }

            ModelState.Remove("Foto");
            ModelState.Remove("autorFoto");

            if (ModelState.IsValid && !fileError)
            {
                try
                {
                    // Attach the 'autor' entity to the context and mark it as modified
                    _context.Update(autor);
                    await _context.SaveChangesAsync();

                    // Process image if a new one was uploaded
                    string localizacaoImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images", "autoresFoto");

                    if (!Directory.Exists(localizacaoImagens))
                    {
                        Directory.CreateDirectory(localizacaoImagens);
                    }

                    if (autorFoto != null && !string.IsNullOrEmpty(newImagePath))
                    {
                        string fullNewImagePath = Path.Combine(localizacaoImagens, newImagePath);
                        using (var streamImage = new FileStream(fullNewImagePath, FileMode.Create))
                        {
                            await autorFoto.CopyToAsync(streamImage);
                        }

                        // Delete old image if a new one was successfully saved and it's different from the old one
                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                        {
                            string fullOldImagePath = Path.Combine(localizacaoImagens, oldImagePath);
                            if (System.IO.File.Exists(fullOldImagePath))
                            {
                                System.IO.File.Delete(fullOldImagePath);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.Id))
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
            return View(autor);
        }

        // GET: Autores/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
                .Include(a => a.ShowsCriados)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // POST: Autores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
                return NotFound();

            // Remove the physical file if it exists
            if (!string.IsNullOrWhiteSpace(autor.Foto))
            {
                var pastaImagens = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images", "autoresFoto"
                );
                var caminhoImagem = Path.Combine(pastaImagens, autor.Foto);

                if (System.IO.File.Exists(caminhoImagem))
                    System.IO.File.Delete(caminhoImagem);
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Helper Method
        private bool AutorExists(int id)
        {
            return _context.Autores.Any(e => e.Id == id);
        }
    }
}