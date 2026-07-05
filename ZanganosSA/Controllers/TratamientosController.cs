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
    public class TratamientosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TratamientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tratamientos
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Tratamientos
                .Include(t => t.ColmenaTratamientos)
                    .ThenInclude(ct => ct.Colmena);
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

        // GET: Tratamientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos
                .Include(t => t.ColmenaTratamientos)
                    .ThenInclude(ct => ct.Colmena)
                        .ThenInclude(c => c!.Apiario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            return View(tratamiento);
        }

        // GET: Tratamientos/Create
        public IActionResult Create()
        {
            ViewBag.Colmenas = _context.Colmenas.Include(c => c.Apiario).OrderBy(c => c.Id).ToList();
            return View();
        }

        // POST: Tratamientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Medicamento,Dosis,FechaInicio,FechaFin,DuracionDias")] Tratamiento tratamiento, int[] selectedColmenas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tratamiento);
                await _context.SaveChangesAsync();

                if (selectedColmenas != null)
                {
                    foreach (var colmenaId in selectedColmenas)
                    {
                        var ct = new ColmenaTratamiento
                        {
                            TratamientoId = tratamiento.Id,
                            ColmenaId = colmenaId,
                            FechaAplicacion = tratamiento.FechaInicio
                        };
                        _context.Add(ct);
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Colmenas = _context.Colmenas.Include(c => c.Apiario).OrderBy(c => c.Id).ToList();
            return View(tratamiento);
        }

        // GET: Tratamientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos
                .Include(t => t.ColmenaTratamientos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null)
            {
                return NotFound();
            }
            ViewBag.Colmenas = _context.Colmenas.Include(c => c.Apiario).OrderBy(c => c.Id).ToList();
            ViewBag.SelectedColmenas = tratamiento.ColmenaTratamientos.Select(ct => ct.ColmenaId).ToList();
            return View(tratamiento);
        }

        // POST: Tratamientos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Medicamento,Dosis,FechaInicio,FechaFin,DuracionDias")] Tratamiento tratamiento, int[] selectedColmenas)
        {
            if (id != tratamiento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tratamiento);
                    await _context.SaveChangesAsync();

                    var existingAssociations = _context.ColmenaTratamientos.Where(ct => ct.TratamientoId == tratamiento.Id);
                    _context.RemoveRange(existingAssociations);
                    await _context.SaveChangesAsync();

                    if (selectedColmenas != null)
                    {
                        foreach (var colmenaId in selectedColmenas)
                        {
                            var ct = new ColmenaTratamiento
                            {
                                TratamientoId = tratamiento.Id,
                                ColmenaId = colmenaId,
                                FechaAplicacion = tratamiento.FechaInicio
                            };
                            _context.Add(ct);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TratamientoExists(tratamiento.Id))
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
            ViewBag.Colmenas = _context.Colmenas.Include(c => c.Apiario).OrderBy(c => c.Id).ToList();
            ViewBag.SelectedColmenas = selectedColmenas?.ToList() ?? new List<int>();
            return View(tratamiento);
        }

        // GET: Tratamientos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos
                .Include(t => t.ColmenaTratamientos)
                    .ThenInclude(ct => ct.Colmena)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            return View(tratamiento);
        }

        // POST: Tratamientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                _context.Tratamientos.Remove(tratamiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TratamientoExists(int id)
        {
            return _context.Tratamientos.Any(e => e.Id == id);
        }

        // POST: Tratamientos/MarcarCompletado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletado(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                // Marcar como completado adelantando la FechaFin a hoy
                tratamiento.FechaFin = DateTime.Today.AddDays(-1);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
