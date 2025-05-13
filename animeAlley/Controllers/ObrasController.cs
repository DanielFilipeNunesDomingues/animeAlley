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
    public class ObrasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObrasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Obras
        public async Task<IActionResult> Index()
        {
            return View(await _context.Obras.ToListAsync());
        }

        // GET: Obras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obras = await _context.Obras
                .FirstOrDefaultAsync(m => m.id == id);
            if (obras == null)
            {
                return NotFound();
            }

            return View(obras);
        }

        // GET: Obras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Obras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,showID")] Obras obras)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obras);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obras);
        }

        // GET: Obras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obras = await _context.Obras.FindAsync(id);
            if (obras == null)
            {
                return NotFound();
            }
            return View(obras);
        }

        // POST: Obras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,showID")] Obras obras)
        {
            if (id != obras.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obras);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObrasExists(obras.id))
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
            return View(obras);
        }

        // GET: Obras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obras = await _context.Obras
                .FirstOrDefaultAsync(m => m.id == id);
            if (obras == null)
            {
                return NotFound();
            }

            return View(obras);
        }

        // POST: Obras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obras = await _context.Obras.FindAsync(id);
            if (obras != null)
            {
                _context.Obras.Remove(obras);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObrasExists(int id)
        {
            return _context.Obras.Any(e => e.id == id);
        }
    }
}
