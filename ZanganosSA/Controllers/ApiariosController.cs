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
    public class ApiariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Apiarios
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Apiarios.AsQueryable();
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

        // GET: Apiarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiario = await _context.Apiarios
                .Include(a => a.Colmenas)
                .Include(a => a.Alimentaciones)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apiario == null)
            {
                return NotFound();
            }

            return View(apiario);
        }

        // GET: Apiarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Apiarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Departamento,SeccionalPolicial,FechaRegistro,Latitud,Longitud")] Apiario apiario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(apiario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(apiario);
        }

        // GET: Apiarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiario = await _context.Apiarios.FindAsync(id);
            if (apiario == null)
            {
                return NotFound();
            }
            return View(apiario);
        }

        // POST: Apiarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Departamento,SeccionalPolicial,FechaRegistro,Latitud,Longitud")] Apiario apiario)
        {
            if (id != apiario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apiario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApiarioExists(apiario.Id))
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
            return View(apiario);
        }

        // GET: Apiarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiario = await _context.Apiarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apiario == null)
            {
                return NotFound();
            }

            return View(apiario);
        }

        // POST: Apiarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiario = await _context.Apiarios.FindAsync(id);
            if (apiario != null)
            {
                _context.Apiarios.Remove(apiario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApiarioExists(int id)
        {
            return _context.Apiarios.Any(e => e.Id == id);
        }

        // GET: Apiarios/ReporteMGAP/5
        public async Task<IActionResult> ReporteMGAP(int? id)
        {
            if (id == null) return NotFound();

            var apiario = await _context.Apiarios
                .Include(a => a.Colmenas)
                    .ThenInclude(c => c.ColmenaTratamientos)
                        .ThenInclude(ct => ct.Tratamiento)
                .Include(a => a.Alimentaciones)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (apiario == null) return NotFound();

            // Cargar cosechas vinculadas a colmenas de este apiario
            var colmenaIds = apiario.Colmenas.Select(c => c.Id).ToList();
            ViewBag.Cosechas = await _context.Cosechas
                .Include(c => c.Barriles)
                .Include(c => c.ColmenaCosechas)
                .Where(c => c.ColmenaCosechas.Any(cc => colmenaIds.Contains(cc.ColmenaId)))
                .ToListAsync();

            return View(apiario);
        }
    }
}
