using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AlfinfData.Models.SQLITE;
using SQLite;

namespace AlfinfData.Services.BdLocal
{
    public class ProduccionRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public ProduccionRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }

        // Método para obtener todas las producciones
        public Task<List<Produccion>> GetAllAsync()
            => _db.Table<Produccion>().ToListAsync();

        // Método que obtiene la lista combinada de jornaleros con sus cajas
        public Task<List<JornaleroConCajas>> GetJornalerosConCajasAsync()
        {
            return _db.QueryAsync<JornaleroConCajas>(
                @"SELECT j.IdJornalero, j.IdCuadrilla, j.Nombre, IFNULL(SUM(p.Cajas), 0) as TotalCajas
          FROM Jornalero j
          LEFT JOIN Produccion p ON j.IdJornalero = p.IdJornalero
          GROUP BY j.IdJornalero, j.IdCuadrilla, j.Nombre
          ORDER BY j.Nombre");
        }
    }
 }
