using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class DiplomesController : Controller
    {
        private readonly AppDbContext _context;

        public DiplomesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Diplomes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Diplomes.ToListAsync());
        }

        // GET: Diplomes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diplome == null)
            {
                return NotFound();
            }

            return View(diplome);
        }

        // GET: Diplomes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diplomes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomDiplome")] Diplome diplome)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diplome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diplome);
        }

        // GET: Diplomes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome == null)
            {
                return NotFound();
            }
            return View(diplome);
        }

        // POST: Diplomes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomDiplome")] Diplome diplome)
        {
            if (id != diplome.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diplome);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiplomeExists(diplome.Id))
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
            return View(diplome);
        }

        // GET: Diplomes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diplome == null)
            {
                return NotFound();
            }

            return View(diplome);
        }

        // POST: Diplomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome != null)
            {
                _context.Diplomes.Remove(diplome);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiplomeExists(int id)
        {
            return _context.Diplomes.Any(e => e.Id == id);
        }
    }
}
