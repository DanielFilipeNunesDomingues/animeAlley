using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;

namespace animeAlley.Controllers
{
    public class AutoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// objeto que contém todas as características do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AutoresController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Autores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Autores.ToListAsync());
        }

        // GET: Autores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // GET: Autores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Autores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,DateNasc,Sobre,Foto")] Autor autor, IFormFile autorFoto)
        {
            bool hasError = false;
            string imagePath = string.Empty;

            if (autorFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de submeter uma Fotografia da Personagem");
            }
            else
            {
                if (autorFoto.ContentType != "image/jpeg" && autorFoto.ContentType != "image/png")
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

                    string extensaoImagem = Path.GetExtension(autorFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;

                    // guardar este nome na BD
                    autor.Foto = imagePath;
                }
            }

            ModelState.Remove("Foto");

            if (ModelState.IsValid && !hasError)
            {
                // Adicionar a personagem à base de dados (o ShowFK já vai ser salvo automaticamente)
                _context.Add(autor);
                await _context.SaveChangesAsync();

                // Salvar a imagem fisicamente
                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                localizacaoImagem = Path.Combine(localizacaoImagem, "images/autoresFoto");
                if (!Directory.Exists(localizacaoImagem))
                {
                    Directory.CreateDirectory(localizacaoImagem);
                }
                // gerar o caminho completo para a imagem
                imagePath = Path.Combine(localizacaoImagem, imagePath);

                // agora, temos condições para guardar a imagem
                using var streamImage = new FileStream(imagePath, FileMode.Create);
                await autorFoto.CopyToAsync(streamImage);

                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }

        // GET: Autores/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DateNasc,Sobre,Foto")] Autor autor, IFormFile autorFoto)
        {
            if (id != autor.Id)
            {
                return NotFound();
            }

            // Se foi enviada uma nova foto, processar
            if (autorFoto != null)
            {
                if (autorFoto.ContentType == "image/jpeg" || autorFoto.ContentType == "image/png")
                {
                    // Remover a foto antiga se existir
                    var personagemExistente = await _context.Personagens.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (personagemExistente != null && !string.IsNullOrWhiteSpace(personagemExistente.Foto))
                    {
                        var pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images", "autoresFoto");
                        var caminhoImagemAntiga = Path.Combine(pastaImagens, personagemExistente.Foto);
                        if (System.IO.File.Exists(caminhoImagemAntiga))
                            System.IO.File.Delete(caminhoImagemAntiga);
                    }

                    // Salvar nova foto
                    Guid g = Guid.NewGuid();
                    string imagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(autorFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;
                    autor.Foto = imagePath;

                    string localizacaoImagem = _webHostEnvironment.WebRootPath;
                    localizacaoImagem = Path.Combine(localizacaoImagem, "images/personagensFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    string caminhoCompleto = Path.Combine(localizacaoImagem, imagePath);
                    using var streamImage = new FileStream(caminhoCompleto, FileMode.Create);
                    await autorFoto.CopyToAsync(streamImage);
                }
            }

            ModelState.Remove("Foto");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1) Obter o registo
            var autor = await _context.Personagens.FindAsync(id);
            if (autor == null)
                return NotFound();

            // 2) Remover o ficheiro físico, se existir
            if (!string.IsNullOrWhiteSpace(autor.Foto))
            {
                // 2.1) Construir o caminho completo até à imagem
                var pastaImagens = Path.Combine(
                    _webHostEnvironment.WebRootPath,       // wwwroot
                    "images", "personagensFoto"            // sub‑pastas
                );
                var caminhoImagem = Path.Combine(pastaImagens, autor.Foto);

                // 2.2) Apagar o ficheiro
                if (System.IO.File.Exists(caminhoImagem))
                    System.IO.File.Delete(caminhoImagem);
            }

            // 3) Remover o registo da BD
            _context.Personagens.Remove(autor);
            await _context.SaveChangesAsync();

            // 4) Voltar à lista
            return RedirectToAction(nameof(Index));
        }

        private bool AutorExists(int id)
        {
            return _context.Autores.Any(e => e.Id == id);
        }
    }
}
