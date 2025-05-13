using AlfinfData.Models.SQLITE;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlfinfData.Services.BdLocal
{
    public class ProduccionRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public ProduccionRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }

        // Insertar un nuevo registro de producción
        public async Task InsertProduccionAsync(Produccion produccion)
        {
            await _db.InsertAsync(produccion);
        }

        // Obtener todas las producciones (opcional)
        public Task<List<Produccion>> GetAllAsync()
        {
            return _db.Table<Produccion>().ToListAsync();
        }

        public Task<List<Produccion>> GetProduccionPorFechaAsync(DateTime fecha)
        {
            return _db.Table<Produccion>()
                      .Where(p => p.Timestamp.Date == fecha.Date)
                      .ToListAsync();
        }
        public Task<List<Produccion>> GetProduccionEntreFechasAsync(DateTime desde, DateTime hasta)
        {
            return _db.Table<Produccion>()
                      .Where(p => p.Timestamp >= desde && p.Timestamp <= hasta)
                      .ToListAsync();
        }


        // Obtener lista combinada de jornaleros con sus cajas totales
        public Task<List<JornaleroConCajas>> GetJornalerosConCajasAsync()
        {
            return _db.QueryAsync<JornaleroConCajas>(
                @"SELECT j.IdJornalero, j.IdCuadrilla, j.Nombre, 
                 IFNULL(SUM(p.Cajas), 0) AS TotalCajas
          FROM Jornalero j
          LEFT JOIN Produccion p ON j.IdJornalero = p.IdJornalero
          WHERE j.Activo = 1
          GROUP BY j.IdJornalero, j.IdCuadrilla, j.Nombre
          ORDER BY j.Nombre");
        }
    }
}
