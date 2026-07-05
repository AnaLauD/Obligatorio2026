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
    public class InspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspeccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspecciones
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Inspecciones
                .Include(i => i.Colmena)
                    .ThenInclude(c => c!.Apiario)
                .OrderByDescending(i => i.FechaHora);
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

        // GET: Inspecciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspecciones
                .Include(i => i.Colmena)
                    .ThenInclude(c => c!.Apiario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // GET: Inspecciones/Create
        public IActionResult Create()
        {
            ViewData["ColmenaId"] = new SelectList(
                _context.Colmenas.Include(c => c.Apiario).Select(c => new { c.Id, Display = c.Id + " - " + c.Apiario!.Departamento }),
                "Id", "Display");
            return View();
        }

        // POST: Inspecciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ColmenaId,FechaHora,EstadoColmena,EstadoReina,PoblacionEstimada,Observaciones")] Inspeccion inspeccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspeccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColmenaId"] = new SelectList(
                _context.Colmenas.Include(c => c.Apiario).Select(c => new { c.Id, Display = c.Id + " - " + c.Apiario!.Departamento }),
                "Id", "Display", inspeccion.ColmenaId);
            return View(inspeccion);
        }

        // GET: Inspecciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspecciones.FindAsync(id);
            if (inspeccion == null)
            {
                return NotFound();
            }
            ViewData["ColmenaId"] = new SelectList(
                _context.Colmenas.Include(c => c.Apiario).Select(c => new { c.Id, Display = c.Id + " - " + c.Apiario!.Departamento }),
                "Id", "Display", inspeccion.ColmenaId);
            return View(inspeccion);
        }

        // POST: Inspecciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ColmenaId,FechaHora,EstadoColmena,EstadoReina,PoblacionEstimada,Observaciones")] Inspeccion inspeccion)
        {
            if (id != inspeccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspeccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspeccionExists(inspeccion.Id))
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
            ViewData["ColmenaId"] = new SelectList(
                _context.Colmenas.Include(c => c.Apiario).Select(c => new { c.Id, Display = c.Id + " - " + c.Apiario!.Departamento }),
                "Id", "Display", inspeccion.ColmenaId);
            return View(inspeccion);
        }

        // GET: Inspecciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspecciones
                .Include(i => i.Colmena)
                    .ThenInclude(c => c!.Apiario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // POST: Inspecciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspeccion = await _context.Inspecciones.FindAsync(id);
            if (inspeccion != null)
            {
                _context.Inspecciones.Remove(inspeccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspeccionExists(int id)
        {
            return _context.Inspecciones.Any(e => e.Id == id);
        }
    }
}
