using AlfinfData.Models.SQLITE;
using SQLite;

namespace AlfinfData.Services.BdLocal
{
    public class HorasRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public HorasRepository(DatabaseService dbService)
        {
            _db = dbService.Conn;
        }

        public async Task<Horas> GetHorasPorJornaleroYFechaAsync(int idJornalero, DateTime fecha)
        {
            return await _db.Table<Horas>()
                .Where(h => h.IdJornalero == idJornalero && h.Fecha == fecha.Date)
                .FirstOrDefaultAsync();
        }
        //Necesario para generar historico
        public async Task<List<JornaleroConHoras>> GetJornalerosConHorasAsync(DateTime fecha)
        {
            var query = @"SELECT h.IdJornalero, j.Nombre, j.IdCuadrilla, h.HN as Hn, h.HE1 as He1, h.HE2 as He2
                  FROM Horas h
                  JOIN Jornalero j ON j.IdJornalero = h.IdJornalero
                  WHERE h.Fecha = ?";
            return await _db.QueryAsync<JornaleroConHoras>(query, fecha.Date);
        }

        //Actualizar e insertar horas.
        public async Task ActualizarHorasAsync(Horas horas)
        {
            await _db.UpdateAsync(horas);
        }
        public async Task InsertarHorasAsync(Horas horas)
        {
            await _db.InsertAsync(horas);
        }
    }
}