using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZanganosSA.Data;
using ZanganosSA.Models;

namespace ZanganosSA.Controllers
{
    public class SimuladorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimuladorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Simulador
        public async Task<IActionResult> Index(int? id)
        {
            // Listar apiarios para el dropdown selector
            var apiarios = await _context.Apiarios
                .OrderBy(a => a.Departamento)
                .ToListAsync();
            
            ViewBag.Apiarios = apiarios;
            ViewBag.SelectedApiarioId = id;

            if (id == null)
            {
                return View();
            }

            var apiario = await _context.Apiarios
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apiario == null)
            {
                return NotFound();
            }

            // Obtener colmenas asociadas con sus tratamientos e inspecciones
            var colmenas = await _context.Colmenas
                .Include(c => c.ColmenaTratamientos)
                    .ThenInclude(ct => ct.Tratamiento)
                .Include(c => c.Inspecciones)
                .Where(c => c.ApiarioId == id)
                .ToListAsync();

            // Obtener alimentaciones del apiario
            var alimentaciones = await _context.Alimentaciones
                .Where(a => a.ApiarioId == id)
                .OrderByDescending(a => a.FechaProgramada)
                .ToListAsync();

            // Obtener cosechas asociadas a las colmenas de este apiario
            var colmenaIds = colmenas.Select(c => c.Id).ToList();
            var cosechas = await _context.Cosechas
                .Include(c => c.ColmenaCosechas)
                .Where(c => c.ColmenaCosechas.Any(cc => colmenaIds.Contains(cc.ColmenaId)))
                .OrderByDescending(c => c.FechaCosecha)
                .ToListAsync();

            // Formatear datos para pasárselos como JSON a la vista (para que JS los procese en tiempo real)
            var colmenasData = colmenas.Select(c => {
                var ultimaInspeccion = c.Inspecciones
                    .OrderByDescending(i => i.FechaHora)
                    .FirstOrDefault();
                
                var ultimoTrat = c.ColmenaTratamientos
                    .OrderByDescending(ct => ct.FechaAplicacion)
                    .FirstOrDefault();

                int? diasDesdeUltimoTratamiento = null;
                string? medicamento = null;
                if (ultimoTrat != null)
                {
                    diasDesdeUltimoTratamiento = (int)(DateTime.Today - ultimoTrat.FechaAplicacion).TotalDays;
                    medicamento = ultimoTrat.Tratamiento?.Medicamento;
                }

                // Si no hay inspección con población estimada, estimar por su estado
                int poblacion = ultimaInspeccion?.PoblacionEstimada ?? c.EstadoColmena switch {
                    "Fuerte" => 50000,
                    "Media" => 35000,
                    "Debil" => 15000,
                    "Critica" => 5000,
                    _ => 25000
                };

                return new {
                    c.Id,
                    Estado = c.EstadoColmena ?? "Media",
                    Reina = c.ReinaEstado ?? "Sana",
                    c.Activa,
                    Poblacion = poblacion,
                    DiasUltimoTratamiento = diasDesdeUltimoTratamiento,
                    UltimoMedicamento = medicamento
                };
            }).ToList();

            var alimentacionesData = alimentaciones.Select(a => new {
                a.TipoAlimento,
                a.FechaProgramada,
                a.Cantidad
            }).ToList();

            var cosechasData = cosechas.Select(c => new {
                c.Id,
                c.FechaCosecha,
                c.KgTotal,
                c.Lote
            }).ToList();

            ViewBag.Apiario = apiario;
            ViewBag.ColmenasJson = System.Text.Json.JsonSerializer.Serialize(colmenasData);
            ViewBag.AlimentacionesJson = System.Text.Json.JsonSerializer.Serialize(alimentacionesData);
            ViewBag.CosechasJson = System.Text.Json.JsonSerializer.Serialize(cosechasData);

            return View();
        }
    }
}
