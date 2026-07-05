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
    public class AlimentacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlimentacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alimentacion
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Alimentaciones
                .Include(a => a.Apiario)
                .OrderByDescending(a => a.FechaProgramada);
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

        // GET: Alimentacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentacion = await _context.Alimentaciones
                .Include(a => a.Apiario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alimentacion == null)
            {
                return NotFound();
            }

            return View(alimentacion);
        }

        // GET: Alimentacion/Create
        public IActionResult Create()
        {
            ViewData["ApiarioId"] = new SelectList(_context.Apiarios, "Id", "Departamento");
            return View();
        }

        // POST: Alimentacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApiarioId,TipoAlimento,FechaProgramada,Cantidad")] Alimentacion alimentacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alimentacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApiarioId"] = new SelectList(_context.Apiarios, "Id", "Departamento", alimentacion.ApiarioId);
            return View(alimentacion);
        }

        // GET: Alimentacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentacion = await _context.Alimentaciones.FindAsync(id);
            if (alimentacion == null)
            {
                return NotFound();
            }
            ViewData["ApiarioId"] = new SelectList(_context.Apiarios, "Id", "Departamento", alimentacion.ApiarioId);
            return View(alimentacion);
        }

        // POST: Alimentacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApiarioId,TipoAlimento,FechaProgramada,Cantidad")] Alimentacion alimentacion)
        {
            if (id != alimentacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alimentacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlimentacionExists(alimentacion.Id))
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
            ViewData["ApiarioId"] = new SelectList(_context.Apiarios, "Id", "Departamento", alimentacion.ApiarioId);
            return View(alimentacion);
        }

        // GET: Alimentacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentacion = await _context.Alimentaciones
                .Include(a => a.Apiario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alimentacion == null)
            {
                return NotFound();
            }

            return View(alimentacion);
        }

        // POST: Alimentacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alimentacion = await _context.Alimentaciones.FindAsync(id);
            if (alimentacion != null)
            {
                _context.Alimentaciones.Remove(alimentacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlimentacionExists(int id)
        {
            return _context.Alimentaciones.Any(e => e.Id == id);
        }

        // POST: Alimentacion/MarcarCompletada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletada(int id)
        {
            var alim = await _context.Alimentaciones.FindAsync(id);
            if (alim != null)
            {
                // Archivar pasándola al historial con fecha de ayer
                alim.FechaProgramada = DateTime.Today.AddDays(-1);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
