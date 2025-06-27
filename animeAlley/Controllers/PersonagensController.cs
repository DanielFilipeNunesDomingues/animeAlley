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
        public async Task<IActionResult> Create([Bind("Id,Nome,TipoPersonagem,Sinopse,PersonagemSexualidade,Idade,DataNasc")] Personagem personagem,
            IFormFile personagemFoto, List<int> selectedShows)
        {
            bool hasError = false;
            string imagePath = string.Empty;

            // Validar foto
            if (personagemFoto == null)
            {
                hasError = true;
                ModelState.AddModelError("personagemFoto", "Tem de submeter uma Fotografia da Personagem");
            }
            else
            {
                if (personagemFoto.ContentType != "image/jpeg" &&
                    personagemFoto.ContentType != "image/png" &&
                    personagemFoto.ContentType != "image/jpg")
                {
                    hasError = true;
                    ModelState.AddModelError("personagemFoto", "Tem de submeter uma Fotografia em formato JPEG ou PNG");
                }
                else if (personagemFoto.Length > 2 * 1024 * 1024) // 2MB
                {
                    hasError = true;
                    ModelState.AddModelError("personagemFoto", "O arquivo é muito grande. Tamanho máximo: 2MB");
                }
                else
                {
                    // Gerar nome único para a imagem
                    Guid g = Guid.NewGuid();
                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    imagePath = g.ToString() + extensaoImagem;
                }
            }

            // Validar shows selecionados
            if (selectedShows == null || !selectedShows.Any())
            {
                hasError = true;
                ModelState.AddModelError("selectedShows", "Selecione pelo menos um show para o personagem");
            }

            // Remover validações que não queremos verificar
            ModelState.Remove("Foto");
            ModelState.Remove("Shows");
            ModelState.Remove("personagemFoto");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    // Definir o caminho da foto no modelo
                    personagem.Foto = imagePath;

                    // Primeiro, adicionar o personagem sem os shows
                    _context.Personagens.Add(personagem);
                    await _context.SaveChangesAsync(); // Salva para obter o ID

                    // Agora buscar os shows selecionados e estabelecer a relação
                    var showsSelecionados = await _context.Shows
                        .Where(s => selectedShows.Contains(s.Id))
                        .ToListAsync();

                    // Estabelecer a relação many-to-many
                    foreach (var show in showsSelecionados)
                    {
                        personagem.Shows.Add(show);
                    }

                    // Salvar as relações
                    await _context.SaveChangesAsync();

                    // Salvar a imagem fisicamente após sucesso da BD
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    string fullImagePath = Path.Combine(localizacaoImagem, imagePath);
                    using (var streamImage = new FileStream(fullImagePath, FileMode.Create))
                    {
                        await personagemFoto.CopyToAsync(streamImage);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Se houve erro, apagar a imagem se foi criada
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        string fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto", imagePath);
                        if (System.IO.File.Exists(fullImagePath))
                        {
                            System.IO.File.Delete(fullImagePath);
                        }
                    }

                    ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", "Detalhes: " + ex.InnerException.Message);
                    }
                }
            }

            // Recarregar a lista de shows em caso de erro
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

            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            var showsAssociados = personagem.Shows.Select(s => s.Id).ToList();
            ViewBag.Shows = new MultiSelectList(shows, "Id", "Nome", showsAssociados);

            return View(personagem);
        }

        // POST: Personagens/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,TipoPersonagem,Sinopse,PersonagemSexualidade,Idade,DataNasc")] Personagem personagem,
            IFormFile personagemFoto, List<int> selectedShows)
        {
            if (id != personagem.Id)
            {
                return NotFound();
            }

            bool hasError = false;
            string newImagePath = string.Empty;

            // Buscar personagem existente
            var existingPersonagem = await _context.Personagens
                .Include(p => p.Shows)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPersonagem == null)
            {
                return NotFound();
            }

            string oldImagePath = existingPersonagem.Foto;

            // Validar nova foto se fornecida
            if (personagemFoto != null)
            {
                if (personagemFoto.ContentType != "image/jpeg" &&
                    personagemFoto.ContentType != "image/png" &&
                    personagemFoto.ContentType != "image/jpg")
                {
                    hasError = true;
                    ModelState.AddModelError("personagemFoto", "A foto deve ser em formato JPEG ou PNG.");
                }
                else if (personagemFoto.Length > 2 * 1024 * 1024)
                {
                    hasError = true;
                    ModelState.AddModelError("personagemFoto", "O arquivo é muito grande. Tamanho máximo: 2MB.");
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    newImagePath = g.ToString() + extensaoImagem;
                }
            }

            // Validar shows selecionados
            if (selectedShows == null || !selectedShows.Any())
            {
                hasError = true;
                ModelState.AddModelError("selectedShows", "Selecione pelo menos um show para o personagem");
            }

            // Remover validações desnecessárias
            ModelState.Remove("Foto");
            ModelState.Remove("personagemFoto");
            ModelState.Remove("Shows");

            if (ModelState.IsValid && !hasError)
            {
                try
                {
                    // Atualizar propriedades básicas
                    existingPersonagem.Nome = personagem.Nome;
                    existingPersonagem.TipoPersonagem = personagem.TipoPersonagem;
                    existingPersonagem.Sinopse = personagem.Sinopse;
                    existingPersonagem.PersonagemSexualidade = personagem.PersonagemSexualidade;
                    existingPersonagem.Idade = personagem.Idade;
                    existingPersonagem.DataNasc = personagem.DataNasc;

                    // Atualizar shows
                    existingPersonagem.Shows.Clear();
                    var showsSelecionados = await _context.Shows
                        .Where(s => selectedShows.Contains(s.Id))
                        .ToListAsync();

                    foreach (var show in showsSelecionados)
                    {
                        existingPersonagem.Shows.Add(show);
                    }

                    // Processar nova imagem se fornecida
                    if (personagemFoto != null && !string.IsNullOrEmpty(newImagePath))
                    {
                        string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto");
                        if (!Directory.Exists(localizacaoImagem))
                        {
                            Directory.CreateDirectory(localizacaoImagem);
                        }

                        // Salvar nova imagem
                        string fullNewImagePath = Path.Combine(localizacaoImagem, newImagePath);
                        using (var streamImage = new FileStream(fullNewImagePath, FileMode.Create))
                        {
                            await personagemFoto.CopyToAsync(streamImage);
                        }

                        // Atualizar caminho da foto
                        existingPersonagem.Foto = newImagePath;

                        // Remover imagem antiga
                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                        {
                            string fullOldImagePath = Path.Combine(localizacaoImagem, oldImagePath);
                            if (System.IO.File.Exists(fullOldImagePath))
                            {
                                System.IO.File.Delete(fullOldImagePath);
                            }
                        }
                    }

                    // Salvar alterações
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
                catch (Exception ex)
                {
                    // Limpar nova imagem se houve erro
                    if (!string.IsNullOrEmpty(newImagePath))
                    {
                        string fullNewImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto", newImagePath);
                        if (System.IO.File.Exists(fullNewImagePath))
                        {
                            System.IO.File.Delete(fullNewImagePath);
                        }
                    }

                    ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", "Detalhes: " + ex.InnerException.Message);
                    }
                }
            }

            // Recarregar dropdown em caso de erro
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