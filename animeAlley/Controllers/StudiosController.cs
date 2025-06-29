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
    public class StudiosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Add IWebHostEnvironment

        public StudiosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Initialize IWebHostEnvironment
        }

        // GET: Studios
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var studios = _context.Studios.Include(a => a.ShowsDesenvolvidos).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                studios = studios.Where(s =>
                    s.Nome.ToLower().Contains(lowerSearch) ||
                    (s.Sobre != null && s.Sobre.ToLower().Contains(lowerSearch))
                );
            }

            return View(await studios.ToListAsync());
        }

        /// <summary>
        /// GET: Studios/Details/5
        /// </summary>
        /// <param name="id">Studio ID</param>
        /// <returns>Details view for the Studio</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos) // Eager load the collection of shows developed by this studio
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // GET: Studios/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Studios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sobre,Fundado,Fechado,Status")] Studio studio, IFormFile studioFoto) // Add IFormFile
        {
            bool hasError = false;
            string imagePath = string.Empty;

            if (studioFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("studioFoto", "Precisa de submeter uma fotografia do Estúdio.");
            }
            else
            {
                // Check content type and file size
                if (studioFoto.ContentType != "image/jpeg" && studioFoto.ContentType != "image/png")
                {
                    hasError = true;
                    ModelState.AddModelError("studioFoto", "A fotografia deve ser em formato JPEG ou PNG.");
                }
                else if (studioFoto.Length > 2 * 1024 * 1024) // 2 MB limit
                {
                    hasError = true;
                    ModelState.AddModelError("studioFoto", "O tamanho do arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    // Process the image
                    Guid g = Guid.NewGuid();
                    imagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(studioFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;
                    studio.Foto = imagePath; // Assign image name to model
                }
            }

            // Remove "Foto" from ModelState to prevent validation issues with null Foto property
            // when IFormFile is handled separately.
            ModelState.Remove("Foto");
            // Also remove the IFormFile itself, as it's not directly part of the model validation
            ModelState.Remove("studioFoto");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    _context.Add(studio);
                    await _context.SaveChangesAsync();

                    // Save the image physically
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "studiosFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    string caminhoCompletoImagem = Path.Combine(localizacaoImagem, imagePath);
                    using var streamImage = new FileStream(caminhoCompletoImagem, FileMode.Create);
                    await studioFoto.CopyToAsync(streamImage);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o estúdio: " + ex.Message);
                }
            }
            // If we got this far, something failed, re-display form
            return View(studio);
        }

        // GET: Studios/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }
            return View(studio);
        }

        // POST: Studios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sobre,Fundado,Fechado,Status,Foto")] Studio studio, IFormFile studioFoto) // Add Foto to bind and IFormFile
        {
            if (id != studio.Id)
            {
                return NotFound();
            }

            bool fileError = false;
            string newImagePath = string.Empty;
            string oldImagePath = string.Empty;

            var existingStudio = await _context.Studios
                .AsNoTracking() // Use AsNoTracking to avoid tracking conflicts when attaching 'studio' later
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingStudio == null)
            {
                return NotFound();
            }

            oldImagePath = existingStudio.Foto;
            studio.Foto = oldImagePath; // Keep the old photo if no new one is uploaded

            if (studioFoto != null)
            {
                if (studioFoto.ContentType != "image/jpeg" && studioFoto.ContentType != "image/png")
                {
                    fileError = true;
                    ModelState.AddModelError("studioFoto", "A fotografia deve ser em formato JPEG ou PNG.");
                }
                else if (studioFoto.Length > 2 * 1024 * 1024) // 2 MB limit
                {
                    fileError = true;
                    ModelState.AddModelError("studioFoto", "O arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    newImagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(studioFoto.FileName).ToLowerInvariant();
                    newImagePath += extensaoImagem;
                    studio.Foto = newImagePath; // Assign new image name to model
                }
            }

            ModelState.Remove("Foto");
            ModelState.Remove("studioFoto");

            if (ModelState.IsValid && !fileError)
            {
                try
                {
                    // Attach the 'studio' entity to the context and mark it as modified
                    _context.Update(studio);
                    await _context.SaveChangesAsync();

                    // Process image if a new one was uploaded
                    string localizacaoImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images", "studiosFoto");

                    if (!Directory.Exists(localizacaoImagens))
                    {
                        Directory.CreateDirectory(localizacaoImagens);
                    }

                    if (studioFoto != null && !string.IsNullOrEmpty(newImagePath))
                    {
                        string fullNewImagePath = Path.Combine(localizacaoImagens, newImagePath);
                        using (var streamImage = new FileStream(fullNewImagePath, FileMode.Create))
                        {
                            await studioFoto.CopyToAsync(streamImage);
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
                    if (!StudioExists(studio.Id))
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
            return View(studio);
        }

        // GET: Studios/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studio = await _context.Studios
                .Include(s => s.ShowsDesenvolvidos) // Include related shows for display in delete confirmation if needed
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studio == null)
            {
                return NotFound();
            }

            return View(studio);
        }

        // POST: Studios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
                return NotFound();

            // Remove the physical file if it exists
            if (!string.IsNullOrWhiteSpace(studio.Foto))
            {
                var pastaImagens = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images", "studiosFoto"
                );
                var caminhoImagem = Path.Combine(pastaImagens, studio.Foto);

                if (System.IO.File.Exists(caminhoImagem))
                    System.IO.File.Delete(caminhoImagem);
            }

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Helper Method
        private bool StudioExists(int id)
        {
            return _context.Studios.Any(e => e.Id == id);
        }
    }
}