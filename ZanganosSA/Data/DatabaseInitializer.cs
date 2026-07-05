using Microsoft.Data.SqlClient;

namespace ZanganosSA.Data
{
    /// <summary>
    /// Detecta si el schema de BaseDeDatos2.sql está aplicado.
    /// Si no, ejecuta el script SQL automáticamente.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString, ILogger logger)
        {
            // 1. Verificar si el schema correcto ya existe
            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                // BaseDeDatos2.sql usa tabla 'Apiario' (singular) con columna 'id_apiario'
                cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Apiario' AND TABLE_SCHEMA = 'dbo'";
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    logger.LogInformation("✅ Schema de BaseDeDatos2 ya existe. La app está lista.");
                    return;
                }
                logger.LogWarning("⚠️ Las tablas de BaseDeDatos2.sql NO existen en la BD. Inicializando...");
            }
            catch (Exception ex)
            {
                logger.LogError("❌ No se pudo conectar a SQL Server: " + ex.Message);
                return;
            }

            // 2. Buscar BaseDeDatos2.sql en ubicaciones conocidas
            var sqlPath = FindSqlScript();
            if (sqlPath == null)
            {
                logger.LogError("❌ No se encontró BaseDeDatos2.sql. Ejecutalo manualmente en SSMS.");
                return;
            }

            logger.LogInformation($"📄 Script encontrado en: {sqlPath}");

            // 3. Ejecutar el script contra SQL Server (con conexión a master para DROP/CREATE DB)
            try
            {
                var masterConnStr = GetMasterConnectionString(connectionString);
                var sql = File.ReadAllText(sqlPath);
                ExecuteSqlScript(masterConnStr, sql, logger);
                logger.LogInformation("✅ Base de datos inicializada correctamente desde BaseDeDatos2.sql");
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Error al ejecutar el script SQL: " + ex.Message);
            }
        }

        private static string GetMasterConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }

        private static string? FindSqlScript()
        {
            var candidates = new[]
            {
                // Relativo al proyecto
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "BaseDeDatos2.sql")),
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "BaseDeDatos2.sql")),
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "BaseDeDatos2.sql")),
                // Ruta absoluta conocida
                @"C:\Users\Usuario\Desktop\Obligatorio 2026\BaseDeDatos2.sql",
            };

            return candidates.FirstOrDefault(File.Exists);
        }

        private static void ExecuteSqlScript(string connectionString, string sql, ILogger logger)
        {
            // Dividir por GO (respetando saltos de línea Windows y Linux)
            var batches = System.Text.RegularExpressions.Regex.Split(sql, @"^\s*GO\s*$",
                System.Text.RegularExpressions.RegexOptions.Multiline |
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string currentDb = "master";

            foreach (var batch in batches)
            {
                var trimmed = batch.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Manejar sentencias USE para cambiar de base de datos
                var useMatch = System.Text.RegularExpressions.Regex.Match(
                    trimmed, @"^\s*USE\s+(\[?[\w]+\]?)\s*;?\s*$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

                if (useMatch.Success)
                {
                    var dbName = useMatch.Groups[1].Value.Trim('[', ']');
                    try
                    {
                        conn.ChangeDatabase(dbName);
                        currentDb = dbName;
                        logger.LogInformation($"   → Usando base de datos: {dbName}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"   ⚠️ No se pudo cambiar a '{dbName}': {ex.Message}");
                    }
                    continue;
                }

                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = trimmed;
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // No fatal - algunos batches pueden fallar si el objeto ya existe
                    logger.LogDebug($"   (non-fatal) [{currentDb}] {ex.Message.Split('\n')[0]}");
                }
            }
        }
    }
}
