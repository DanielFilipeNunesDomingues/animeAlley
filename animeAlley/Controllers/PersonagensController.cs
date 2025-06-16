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

        /// <summary>
        /// objeto que contém todas as características do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonagensController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Personagens
        public async Task<IActionResult> Index()
        {
            // Incluir o show associado para mostrar na listagem
            var personagens = await _context.Personagens
                .Include(p => p.Show)
                .ToListAsync();

            return View(personagens);
        }

        // GET: Personagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Incluir o show associado nos detalhes
            var personagem = await _context.Personagens
                .Include(p => p.Show)
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
            // Buscar todos os shows para popular o dropdown
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new SelectList(shows, "Id", "Nome");

            return View();
        }

        // POST: Personagens/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,TipoPersonagem,Sinopse,Foto,ShowFK")] Personagem personagem, IFormFile personagemFoto)
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

                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;

                    // guardar este nome na BD
                    personagem.Foto = imagePath;
                }
            }

            ModelState.Remove("Foto");

            if (ModelState.IsValid && !hasError)
            {
                // Adicionar a personagem à base de dados (o ShowFK já vai ser salvo automaticamente)
                _context.Add(personagem);
                await _context.SaveChangesAsync();

                // Salvar a imagem fisicamente
                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                localizacaoImagem = Path.Combine(localizacaoImagem, "images/personagensFoto");
                if (!Directory.Exists(localizacaoImagem))
                {
                    Directory.CreateDirectory(localizacaoImagem);
                }
                // gerar o caminho completo para a imagem
                imagePath = Path.Combine(localizacaoImagem, imagePath);

                // agora, temos condições para guardar a imagem
                using var streamImage = new FileStream(imagePath, FileMode.Create);
                await personagemFoto.CopyToAsync(streamImage);

                return RedirectToAction(nameof(Index));
            }

            // Se chegou aqui, algo correu mal, recarregar a lista de shows
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new SelectList(shows, "Id", "Nome", personagem.ShowFK);

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

            var personagem = await _context.Personagens.FindAsync(id);

            if (personagem == null)
            {
                return NotFound();
            }

            // Buscar todos os shows para o dropdown
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new SelectList(shows, "Id", "Nome", personagem.ShowFK);

            return View(personagem);
        }

        // POST: Personagens/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,TipoPersonagem,Sinopse,Foto,ShowFK")] Personagem personagem, IFormFile personagemFoto)
        {
            if (id != personagem.Id)
            {
                return NotFound();
            }

            // Se foi enviada uma nova foto, processar
            if (personagemFoto != null)
            {
                if (personagemFoto.ContentType == "image/jpeg" || personagemFoto.ContentType == "image/png")
                {
                    // Remover a foto antiga se existir
                    var personagemExistente = await _context.Personagens.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (personagemExistente != null && !string.IsNullOrWhiteSpace(personagemExistente.Foto))
                    {
                        var pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personagensFoto");
                        var caminhoImagemAntiga = Path.Combine(pastaImagens, personagemExistente.Foto);
                        if (System.IO.File.Exists(caminhoImagemAntiga))
                            System.IO.File.Delete(caminhoImagemAntiga);
                    }

                    // Salvar nova foto
                    Guid g = Guid.NewGuid();
                    string imagePath = g.ToString();
                    string extensaoImagem = Path.GetExtension(personagemFoto.FileName).ToLowerInvariant();
                    imagePath += extensaoImagem;
                    personagem.Foto = imagePath;

                    string localizacaoImagem = _webHostEnvironment.WebRootPath;
                    localizacaoImagem = Path.Combine(localizacaoImagem, "images/personagensFoto");
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    string caminhoCompleto = Path.Combine(localizacaoImagem, imagePath);
                    using var streamImage = new FileStream(caminhoCompleto, FileMode.Create);
                    await personagemFoto.CopyToAsync(streamImage);
                }
            }

            ModelState.Remove("Foto");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personagem);
                    await _context.SaveChangesAsync();
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

            // Se chegou aqui, recarregar dropdown
            var shows = await _context.Shows
                .Select(s => new { s.Id, s.Nome })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            ViewBag.Shows = new SelectList(shows, "Id", "Nome", personagem.ShowFK);

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
                .Include(p => p.Show)
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
            // 1) Obter o registo
            var personagem = await _context.Personagens.FindAsync(id);
            if (personagem == null)
                return NotFound();

            // 2) Remover o ficheiro físico, se existir
            if (!string.IsNullOrWhiteSpace(personagem.Foto))
            {
                // 2.1) Construir o caminho completo até à imagem
                var pastaImagens = Path.Combine(
                    _webHostEnvironment.WebRootPath,       // wwwroot
                    "images", "personagensFoto"            // sub‑pastas
                );
                var caminhoImagem = Path.Combine(pastaImagens, personagem.Foto);

                // 2.2) Apagar o ficheiro
                if (System.IO.File.Exists(caminhoImagem))
                    System.IO.File.Delete(caminhoImagem);
            }

            // 3) Remover o registo da BD
            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();

            // 4) Voltar à lista
            return RedirectToAction(nameof(Index));
        }

        private bool PersonagemExists(int id)
        {
            return _context.Personagens.Any(e => e.Id == id);
        }
    }
}