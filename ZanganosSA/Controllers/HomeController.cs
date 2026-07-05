using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZanganosSA.Data;
using ZanganosSA.Models;

namespace ZanganosSA.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var hoy = DateTime.Today;

            var viewModel = new HomeViewModel
            {
                TotalApiarios      = await _context.Apiarios.CountAsync(),
                TotalColmenas      = await _context.Colmenas.CountAsync(),
                ColmenasActivas    = await _context.Colmenas.CountAsync(c => c.Activa),
                ColmenasInactivas  = await _context.Colmenas.CountAsync(c => !c.Activa),
                TotalInspecciones  = await _context.Inspecciones.CountAsync(),
                ProduccionTotalKg  = await _context.Cosechas.SumAsync(c => c.KgTotal),

                // Tratamientos sanitarios realmente activos hoy (FechaInicio <= hoy y FechaFin >= hoy)
                TratamientosActivos = await _context.Tratamientos
                    .Include(t => t.ColmenaTratamientos)
                        .ThenInclude(ct => ct.Colmena)
                    .Where(t => t.FechaInicio <= hoy && t.FechaFin >= hoy)
                    .OrderBy(t => t.FechaFin)
                    .ToListAsync(),

                // Alimentaciones programadas futuras (>= hoy)
                ProximasAlimentaciones = await _context.Alimentaciones
                    .Include(a => a.Apiario)
                    .Where(a => a.FechaProgramada >= hoy)
                    .OrderBy(a => a.FechaProgramada)
                    .Take(8)
                    .ToListAsync()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron cargar datos del dashboard.");
            return View(new HomeViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
