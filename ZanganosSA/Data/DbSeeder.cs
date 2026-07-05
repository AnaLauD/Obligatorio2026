using Microsoft.EntityFrameworkCore;
using ZanganosSA.Models;

namespace ZanganosSA.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Verificar conexión a la BD existente (creada por el script SQL)
            // NO borrar ni recrear la base de datos
            // Si ya hay datos, no insertar nada
            if (context.Apiarios.Any())
            {
                return; // La BD ya tiene datos (del script SQL o inserts manuales)
            }

            // Si la BD está vacía, insertar los mismos datos del script BaseDeDatos2.sql
            // Esto es un fallback por si alguien no ejecutó el script SQL

            // 1. Seed Apiarios (coincide con INSERT INTO Apiario del SQL)
            context.Apiarios.AddRange(
                new Apiario { Id = 1, Departamento = "Colonia", SeccionalPolicial = "5ta Seccional", FechaRegistro = new DateTime(2025, 04, 10), Latitud = -34.46260000m, Longitud = -57.84000000m },
                new Apiario { Id = 2, Departamento = "San José", SeccionalPolicial = "3ra Seccional", FechaRegistro = new DateTime(2024, 08, 15), Latitud = -34.33672100m, Longitud = -56.71453200m },
                new Apiario { Id = 3, Departamento = "Canelones", SeccionalPolicial = "1ra Seccional", FechaRegistro = new DateTime(2024, 10, 01), Latitud = -34.52289100m, Longitud = -56.01874500m },
                new Apiario { Id = 4, Departamento = "Flores", SeccionalPolicial = "2da Seccional", FechaRegistro = new DateTime(2025, 01, 20), Latitud = -33.52801200m, Longitud = -56.87543200m }
            );
            context.SaveChanges();

            // 2. Seed Colmenas (coincide con INSERT INTO Colmena del SQL)
            context.Colmenas.AddRange(
                new Colmena { Id = 101, ApiarioId = 1, TipoCaja = "Langstroth", EstadoColmena = "Media", ReinaEstado = "Vieja", Activa = true, Observaciones = "Requiere cambio de reina pronto" },
                new Colmena { Id = 102, ApiarioId = 1, TipoCaja = "Langstroth", EstadoColmena = "Fuerte", ReinaEstado = "Joven", Activa = true, Observaciones = "Colmena productiva, excelente postura" },
                new Colmena { Id = 103, ApiarioId = 2, TipoCaja = "Langstroth", EstadoColmena = "Media", ReinaEstado = "Sana", Activa = true, Observaciones = "Estado normal, bajo monitoreo" },
                new Colmena { Id = 104, ApiarioId = 2, TipoCaja = "Cuadro", EstadoColmena = "Debil", ReinaEstado = "Vieja", Activa = true, Observaciones = "Requiere refuerzo de población" },
                new Colmena { Id = 105, ApiarioId = 3, TipoCaja = "Langstroth", EstadoColmena = "Fuerte", ReinaEstado = "Joven", Activa = true, Observaciones = "Nueva colmena temporada 2025" },
                new Colmena { Id = 106, ApiarioId = 3, TipoCaja = "Langstroth", EstadoColmena = "Critica", ReinaEstado = "Ausente", Activa = false, Observaciones = "Sin reina, posible enjambrazón" },
                new Colmena { Id = 107, ApiarioId = 4, TipoCaja = "Langstroth", EstadoColmena = "Fuerte", ReinaEstado = "Sana", Activa = true, Observaciones = "Apiario Norte, buena florada cercana" }
            );
            context.SaveChanges();

            // 3. Seed Inspecciones (coincide con INSERT INTO Inspeccion del SQL)
            context.Inspecciones.AddRange(
                new Inspeccion { Id = 501, ColmenaId = 101, FechaHora = new DateTime(2026, 05, 10, 09, 30, 00), EstadoColmena = "Fuerte", EstadoReina = "Sana", PoblacionEstimada = 45000, Observaciones = "Buena postura de huevos, sin signos de varroa" },
                new Inspeccion { Id = 502, ColmenaId = 102, FechaHora = new DateTime(2026, 05, 10, 10, 00, 00), EstadoColmena = "Fuerte", EstadoReina = "Joven", PoblacionEstimada = 55000, Observaciones = "Excelente estado, cuadros llenos de miel" },
                new Inspeccion { Id = 503, ColmenaId = 103, FechaHora = new DateTime(2026, 05, 11, 08, 30, 00), EstadoColmena = "Media", EstadoReina = "Sana", PoblacionEstimada = 32000, Observaciones = "Estado normal" },
                new Inspeccion { Id = 504, ColmenaId = 106, FechaHora = new DateTime(2026, 05, 11, 09, 15, 00), EstadoColmena = "Critica", EstadoReina = "Ausente", PoblacionEstimada = 8000, Observaciones = "Sin reina detectada" },
                new Inspeccion { Id = 505, ColmenaId = 107, FechaHora = new DateTime(2026, 05, 12, 07, 45, 00), EstadoColmena = "Fuerte", EstadoReina = "Sana", PoblacionEstimada = 48000, Observaciones = "Muy buena actividad" }
            );
            context.SaveChanges();

            // 4. Seed Cosechas (coincide con INSERT INTO Cosecha del SQL)
            context.Cosechas.AddRange(
                new Cosecha { Id = 301, FechaCosecha = new DateTime(2026, 05, 20), KgTotal = 250.50m, Lote = "LOTE-2026-A", Observaciones = "Miel de pradera con excelente densidad" },
                new Cosecha { Id = 302, FechaCosecha = new DateTime(2025, 11, 15), KgTotal = 380.00m, Lote = "LOTE-2025-B", Observaciones = "Cosecha de primavera, miel multifloral" },
                new Cosecha { Id = 303, FechaCosecha = new DateTime(2026, 04, 10), KgTotal = 420.50m, Lote = "LOTE-2026-B", Observaciones = "Mejor cosecha histórica del apiario 1" },
                new Cosecha { Id = 304, FechaCosecha = new DateTime(2026, 05, 25), KgTotal = 195.00m, Lote = "LOTE-2026-C", Observaciones = "Cosecha parcial apiario 3" }
            );
            context.SaveChanges();

            // 5. Seed Barriles (coincide con INSERT INTO Barril_exportacion del SQL)
            context.Barriles.AddRange(
                new Barril { Id = 401, CosechaId = 301, PesoKg = 120.00m, Estado = "Listo", Destino = "Argentina" },
                new Barril { Id = 402, CosechaId = 301, PesoKg = 130.50m, Estado = "Listo", Destino = "Argentina" },
                new Barril { Id = 403, CosechaId = 302, PesoKg = 295.00m, Estado = "Exportado", Destino = "Alemania" },
                new Barril { Id = 404, CosechaId = 302, PesoKg = 85.00m, Estado = "Reservado", Destino = "Alemania" },
                new Barril { Id = 405, CosechaId = 303, PesoKg = 300.00m, Estado = "Listo", Destino = "Estados Unidos" },
                new Barril { Id = 406, CosechaId = 303, PesoKg = 120.50m, Estado = "Listo", Destino = "JapOn" }
            );
            context.SaveChanges();

            // 6. Seed Alimentacion (coincide con INSERT INTO Alimentacion del SQL)
            context.Alimentaciones.AddRange(
                new Alimentacion { Id = 701, ApiarioId = 1, TipoAlimento = "Jarabe", FechaProgramada = new DateTime(2026, 06, 01), Cantidad = 5.00m },
                new Alimentacion { Id = 702, ApiarioId = 1, TipoAlimento = "Jarabe de azúcar", FechaProgramada = new DateTime(2026, 06, 15), Cantidad = 8.00m },
                new Alimentacion { Id = 703, ApiarioId = 2, TipoAlimento = "Candi", FechaProgramada = new DateTime(2026, 06, 10), Cantidad = 4.50m },
                new Alimentacion { Id = 704, ApiarioId = 3, TipoAlimento = "Polen sustituto", FechaProgramada = new DateTime(2026, 06, 20), Cantidad = 3.00m }
            );
            context.SaveChanges();

            // 7. Seed Tratamientos (coincide con INSERT INTO Tratam_sanitarios del SQL)
            context.Tratamientos.AddRange(
                new Tratamiento { Id = 801, Medicamento = "Timol", Dosis = "50ml por colmena", FechaInicio = new DateTime(2026, 05, 01), FechaFin = new DateTime(2026, 05, 15), DuracionDias = 14 },
                new Tratamiento { Id = 802, Medicamento = "Apivar", Dosis = "2 tiras por colmena", FechaInicio = new DateTime(2026, 04, 01), FechaFin = new DateTime(2026, 05, 27), DuracionDias = 56 },
                new Tratamiento { Id = 803, Medicamento = "Oxabiol", Dosis = "5ml por colmena", FechaInicio = new DateTime(2026, 05, 15), FechaFin = new DateTime(2026, 05, 18), DuracionDias = 3 },
                new Tratamiento { Id = 804, Medicamento = "Api-Bioxal", Dosis = "3g por colmena", FechaInicio = new DateTime(2026, 05, 20), FechaFin = new DateTime(2026, 05, 21), DuracionDias = 1 }
            );
            context.SaveChanges();

            // 8. Seed Cosecha_Colmena (coincide con INSERT INTO Cosecha_Colmena del SQL)
            context.ColmenaCosechas.AddRange(
                new ColmenaCosecha { CosechaId = 301, ColmenaId = 101 },
                new ColmenaCosecha { CosechaId = 302, ColmenaId = 101 },
                new ColmenaCosecha { CosechaId = 302, ColmenaId = 102 },
                new ColmenaCosecha { CosechaId = 302, ColmenaId = 103 },
                new ColmenaCosecha { CosechaId = 303, ColmenaId = 101 },
                new ColmenaCosecha { CosechaId = 303, ColmenaId = 102 },
                new ColmenaCosecha { CosechaId = 303, ColmenaId = 105 },
                new ColmenaCosecha { CosechaId = 304, ColmenaId = 105 },
                new ColmenaCosecha { CosechaId = 304, ColmenaId = 106 }
            );
            context.SaveChanges();

            // 9. Seed Colmena_Tratamiento (coincide con INSERT INTO Colmena_Tratamiento del SQL)
            context.ColmenaTratamientos.AddRange(
                new ColmenaTratamiento { ColmenaId = 101, TratamientoId = 801, FechaAplicacion = new DateTime(2026, 05, 01) },
                new ColmenaTratamiento { ColmenaId = 101, TratamientoId = 802, FechaAplicacion = new DateTime(2026, 04, 01) },
                new ColmenaTratamiento { ColmenaId = 102, TratamientoId = 802, FechaAplicacion = new DateTime(2026, 04, 01) },
                new ColmenaTratamiento { ColmenaId = 103, TratamientoId = 803, FechaAplicacion = new DateTime(2026, 05, 15) },
                new ColmenaTratamiento { ColmenaId = 104, TratamientoId = 803, FechaAplicacion = new DateTime(2026, 05, 15) },
                new ColmenaTratamiento { ColmenaId = 105, TratamientoId = 804, FechaAplicacion = new DateTime(2026, 05, 20) },
                new ColmenaTratamiento { ColmenaId = 106, TratamientoId = 804, FechaAplicacion = new DateTime(2026, 05, 20) },
                new ColmenaTratamiento { ColmenaId = 107, TratamientoId = 801, FechaAplicacion = new DateTime(2026, 05, 01) }
            );
            context.SaveChanges();
        }
    }
}
