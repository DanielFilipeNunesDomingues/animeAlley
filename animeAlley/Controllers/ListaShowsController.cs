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
    public class ListaShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListaShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ListaShows
        public async Task<IActionResult> Index()
        {
            return View(await _context.ListaShows.ToListAsync());
        }

        // GET: ListaShows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaShows = await _context.ListaShows
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listaShows == null)
            {
                return NotFound();
            }

            return View(listaShows);
        }

        // GET: ListaShows/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ListaShows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,status,listaId,showId")] ListaShows listaShows)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listaShows);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(listaShows);
        }

        // GET: ListaShows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaShows = await _context.ListaShows.FindAsync(id);
            if (listaShows == null)
            {
                return NotFound();
            }
            return View(listaShows);
        }

        // POST: ListaShows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,status,listaId,showId")] ListaShows listaShows)
        {
            if (id != listaShows.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listaShows);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListaShowsExists(listaShows.Id))
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
            return View(listaShows);
        }

        // GET: ListaShows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaShows = await _context.ListaShows
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listaShows == null)
            {
                return NotFound();
            }

            return View(listaShows);
        }

        // POST: ListaShows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listaShows = await _context.ListaShows.FindAsync(id);
            if (listaShows != null)
            {
                _context.ListaShows.Remove(listaShows);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListaShowsExists(int id)
        {
            return _context.ListaShows.Any(e => e.Id == id);
        }
    }
}
