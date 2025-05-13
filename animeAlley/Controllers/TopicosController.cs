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
    public class TopicosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Topicos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Topicos.ToListAsync());
        }

        // GET: Topicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicos = await _context.Topicos
                .FirstOrDefaultAsync(m => m.id == id);
            if (topicos == null)
            {
                return NotFound();
            }

            return View(topicos);
        }

        // GET: Topicos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Topicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,titulo,comentario,dataPost,utilizadorId,comentariosID")] Topicos topicos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topicos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topicos);
        }

        // GET: Topicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicos = await _context.Topicos.FindAsync(id);
            if (topicos == null)
            {
                return NotFound();
            }
            return View(topicos);
        }

        // POST: Topicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,titulo,comentario,dataPost,utilizadorId,comentariosID")] Topicos topicos)
        {
            if (id != topicos.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topicos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicosExists(topicos.id))
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
            return View(topicos);
        }

        // GET: Topicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicos = await _context.Topicos
                .FirstOrDefaultAsync(m => m.id == id);
            if (topicos == null)
            {
                return NotFound();
            }

            return View(topicos);
        }

        // POST: Topicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topicos = await _context.Topicos.FindAsync(id);
            if (topicos != null)
            {
                _context.Topicos.Remove(topicos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopicosExists(int id)
        {
            return _context.Topicos.Any(e => e.id == id);
        }
    }
}
