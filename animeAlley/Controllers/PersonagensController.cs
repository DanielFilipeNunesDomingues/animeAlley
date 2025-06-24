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
using Microsoft.AspNetCore.Authorization;

namespace animeAlley.Controllers
{
    public class PersonagensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonagensController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Personagens
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var personagem = from p in _context.Personagens.Include(p => p.Shows)
                             select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                personagem = personagem.Where(p =>
                    p.Nome.ToLower().Contains(lowerSearch) ||
                    p.Sinopse.ToLower().Contains(lowerSearch) ||
                    p.Shows.Any(s => s.Nome.ToLower().Contains(lowerSearch))
                );
            }

            return View(await personagem.ToListAsync());
        }

        // GET: Personagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Incluir os shows associados nos detalhes
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personagem == null)
            {
                return NotFound();
            }

            return View(personagem);
        }

        // GET: Personagens/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Buscar todos os shows para popular o dropdown múltiplo
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new MultiSelectList(shows, "Id", "Nome");

            return View();
        }

        // POST: Personagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,TipoPersonagem,Sinopse,Foto,PersonagemSexualidade,Idade,DataNasc")] Personagem personagem,
            IFormFile personagemFoto, List<int> selectedShows)
        {
            bool hasError = false;
            string imagePath = string.Empty;

            if (personagemFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("", "Tem de submeter uma Fotografia da Personagem");
            }
            else
            {
                if (personagemFoto.ContentType != "image/jpeg" && personagemFoto.ContentType != "image/png")
                {
                    hasError = true;
                    ModelState.AddModelError("", "Tem de submeter uma Fotografia");
                }
                else
                {
                    // Processar a imagem
                    Guid g = Guid.NewGuid();
                    imagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;
                    personagem.Foto = imagePath;
                }
            }

            // Validar se pelo menos um show foi selecionado
            if (selectedShows == null || !selectedShows.Any())
            {
                hasError = true;
                ModelState.AddModelError("", "Selecione pelo menos um show para o personagem");
            }

            ModelState.Remove("Foto");
            ModelState.Remove("Shows");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    // Buscar os shows selecionados
                    var showsSelecionados = await _context.Shows
                        .Where(s => selectedShows.Contains(s.Id))
                        .ToListAsync();

                    // Associar os shows ao personagem
                    personagem.Shows = showsSelecionados;

                    // Adicionar a personagem à base de dados
                    _context.Add(personagem);
                    await _context.SaveChangesAsync();

                    // Salvar a imagem fisicamente
                    string localizacaoImagem = _webHostEnvironment.WebRootPath;
                    localizacaoImagem = Path.Combine(localizacaoImagem, "images/personagensFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    imagePath = Path.Combine(localizacaoImagem, imagePath);
                    using var streamImage = new FileStream(imagePath, FileMode.Create);
                    await personagemFoto.CopyToAsync(streamImage);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                }
            }

            // Se chegou aqui, algo correu mal, recarregar a lista de shows
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new MultiSelectList(shows, "Id", "Nome", selectedShows);

            return View(personagem);
        }

        // GET: Personagens/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null)
            {
                return NotFound();
            }

            // Buscar todos os shows para o dropdown
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            // IDs dos shows já associados ao personagem
            var showsAssociados = personagem.Shows.Select(s => s.Id).ToList();

            ViewBag.Shows = new MultiSelectList(shows, "Id", "Nome", showsAssociados);

            return View(personagem);
        }

        // POST: Personagens/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,TipoPersonagem,Sinopse,Foto,PersonagemSexualidade,Idade,DataNasc")] Personagem personagem,
            IFormFile personagemFoto, List<int> selectedShows)
        {
            if (id != personagem.Id)
            {
                return NotFound();
            }

            bool fileError = false;
            string newImagePath = string.Empty;
            string oldImagePath = string.Empty;

            var existingPersonagem = await _context.Personagens
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPersonagem == null)
            {
                return NotFound();
            }

            oldImagePath = existingPersonagem.Foto;
            personagem.Foto = oldImagePath;

            if (personagemFoto != null)
            {
                if (personagemFoto.ContentType != "image/jpeg" && personagemFoto.ContentType != "image/png")
                {
                    fileError = true;
                    ModelState.AddModelError("personagemFoto", "A foto deve ser em formato JPEG ou PNG.");
                }
                else if (personagemFoto.Length > 2 * 1024 * 1024)
                {
                    fileError = true;
                    ModelState.AddModelError("personagemFoto", "O arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    newImagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    newImagePath += extensaoImagem;
                    personagem.Foto = newImagePath;
                }
            }

            // Validar se pelo menos um show foi selecionado
            if (selectedShows == null || !selectedShows.Any())
            {
                fileError = true;
                ModelState.AddModelError("", "Selecione pelo menos um show para o personagem");
            }

            ModelState.Remove("Foto");
            ModelState.Remove("personagemFoto");
            ModelState.Remove("Shows");

            if (ModelState.IsValid && !fileError)
            {
                try
                {
                    // Buscar o personagem com os shows atuais
                    var personagemComShows = await _context.Personagens
                        .Include(p => p.Shows)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (personagemComShows == null)
                    {
                        return NotFound();
                    }

                    // Atualizar propriedades básicas
                    personagemComShows.Nome = personagem.Nome;
                    personagemComShows.TipoPersonagem = personagem.TipoPersonagem;
                    personagemComShows.Sinopse = personagem.Sinopse;
                    personagemComShows.PersonagemSexualidade = personagem.PersonagemSexualidade;
                    personagemComShows.Idade = personagem.Idade;
                    personagemComShows.DataNasc = personagem.DataNasc;
                    personagemComShows.Foto = personagem.Foto;

                    // Atualizar associações com shows
                    personagemComShows.Shows.Clear();
                    var showsSelecionados = await _context.Shows
                        .Where(s => selectedShows.Contains(s.Id))
                        .ToListAsync();

                    foreach (var show in showsSelecionados)
                    {
                        personagemComShows.Shows.Add(show);
                    }

                    await _context.SaveChangesAsync();

                    // Processar imagem se necessário
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto");

                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    if (personagemFoto != null && !string.IsNullOrEmpty(newImagePath))
                    {
                        string fullNewImagePath = Path.Combine(localizacaoImagem, newImagePath);
                        using var streamImage = new FileStream(fullNewImagePath, FileMode.Create);
                        await personagemFoto.CopyToAsync(streamImage);

                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                        {
                            string fullOldImagePath = Path.Combine(localizacaoImagem, oldImagePath);
                            if (System.IO.File.Exists(fullOldImagePath))
                            {
                                System.IO.File.Delete(fullOldImagePath);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonagemExists(personagem.Id))
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

            // Se chegou aqui, houve erro. Recarregar dropdown
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new MultiSelectList(shows, "Id", "Nome", selectedShows);

            return View(personagem);
        }

        // GET: Personagens/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personagem == null)
            {
                return NotFound();
            }

            return View(personagem);
        }

        // POST: Personagens/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personagem == null)
                return NotFound();

            // Remover o ficheiro físico, se existir
            if (!string.IsNullOrWhiteSpace(personagem.Foto))
            {
                var pastaImagens = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images", "personagensFoto"
                );
                var caminhoImagem = Path.Combine(pastaImagens, personagem.Foto);

                if (System.IO.File.Exists(caminhoImagem))
                    System.IO.File.Delete(caminhoImagem);
            }

            // As associações N-M serão removidas automaticamente pelo EF Core
            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PersonagemExists(int id)
        {
            return _context.Personagens.Any(e => e.Id == id);
        }
    }
}