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
    public class PersonagensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Personagens
        public async Task<IActionResult> Index()
        {
            return View(await _context.Personagens.ToListAsync());
        }

        // GET: Personagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagens = await _context.Personagens
                .FirstOrDefaultAsync(m => m.id == id);
            if (personagens == null)
            {
                return NotFound();
            }

            return View(personagens);
        }

        // GET: Personagens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personagens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nome,tipoPersonagem,sinopse,foto,obras")] Personagens personagens)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personagens);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personagens);
        }

        // GET: Personagens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagens = await _context.Personagens.FindAsync(id);
            if (personagens == null)
            {
                return NotFound();
            }
            return View(personagens);
        }

        // POST: Personagens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nome,tipoPersonagem,sinopse,foto,obras")] Personagens personagens)
        {
            if (id != personagens.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personagens);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonagensExists(personagens.id))
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
            return View(personagens);
        }

        // GET: Personagens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagens = await _context.Personagens
                .FirstOrDefaultAsync(m => m.id == id);
            if (personagens == null)
            {
                return NotFound();
            }

            return View(personagens);
        }

        // POST: Personagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personagens = await _context.Personagens.FindAsync(id);
            if (personagens != null)
            {
                _context.Personagens.Remove(personagens);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonagensExists(int id)
        {
            return _context.Personagens.Any(e => e.id == id);
        }
    }
}
