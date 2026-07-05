using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZanganosSA.Data;
using ZanganosSA.Models;

namespace ZanganosSA.Controllers
{
    public class BarrilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarrilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Barriles
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Barriles.Include(b => b.Cosecha);
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(items);
        }

        // GET: Barriles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barril = await _context.Barriles
                .Include(b => b.Cosecha)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (barril == null)
            {
                return NotFound();
            }

            return View(barril);
        }

        // GET: Barriles/Create
        public IActionResult Create()
        {
            ViewData["CosechaId"] = new SelectList(_context.Cosechas, "Id", "Lote");
            return View();
        }

        // POST: Barriles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CosechaId,PesoKg,Estado,Destino")] Barril barril)
        {
            if (ModelState.IsValid)
            {
                _context.Add(barril);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CosechaId"] = new SelectList(_context.Cosechas, "Id", "Lote", barril.CosechaId);
            return View(barril);
        }

        // GET: Barriles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barril = await _context.Barriles.FindAsync(id);
            if (barril == null)
            {
                return NotFound();
            }
            ViewData["CosechaId"] = new SelectList(_context.Cosechas, "Id", "Lote", barril.CosechaId);
            return View(barril);
        }

        // POST: Barriles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CosechaId,PesoKg,Estado,Destino")] Barril barril)
        {
            if (id != barril.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(barril);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarrilExists(barril.Id))
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
            ViewData["CosechaId"] = new SelectList(_context.Cosechas, "Id", "Lote", barril.CosechaId);
            return View(barril);
        }

        // GET: Barriles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barril = await _context.Barriles
                .Include(b => b.Cosecha)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (barril == null)
            {
                return NotFound();
            }

            return View(barril);
        }

        // POST: Barriles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barril = await _context.Barriles.FindAsync(id);
            if (barril != null)
            {
                _context.Barriles.Remove(barril);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Barriles/Trazabilidad/5
        public async Task<IActionResult> Trazabilidad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barril = await _context.Barriles
                .Include(b => b.Cosecha)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (barril == null)
            {
                return NotFound();
            }

            // Obtener las colmenas que aportaron a esta cosecha, con apiarios, tratamientos e inspecciones
            var colmenas = await _context.ColmenaCosechas
                .Where(cc => cc.CosechaId == barril.CosechaId)
                .Include(cc => cc.Colmena)
                    .ThenInclude(c => c!.Apiario)
                .Include(cc => cc.Colmena)
                    .ThenInclude(c => c!.ColmenaTratamientos)
                        .ThenInclude(ct => ct.Tratamiento)
                .Include(cc => cc.Colmena)
                    .ThenInclude(c => c!.Inspecciones)
                .Select(cc => cc.Colmena)
                .ToListAsync();

            ViewBag.ColmenasInvolucradas = colmenas;

            return View(barril);
        }

        private bool BarrilExists(int id)
        {
            return _context.Barriles.Any(e => e.Id == id);
        }
    }
}
