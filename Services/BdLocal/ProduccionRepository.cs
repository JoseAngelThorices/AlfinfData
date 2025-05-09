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

        public async Task UpdateAsync(JornaleroConCajas jornalero)
        {
            await _db.UpdateAsync(jornalero);
        }
        public async Task InsertProduccionAsync(int idJornalero, int cajas)
        {
            var produccion = new Produccion
            {
                IdJornalero = idJornalero,
                Cajas = cajas,
                Timestamp = DateTime.Now
            };

            await _db.InsertAsync(produccion);
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
