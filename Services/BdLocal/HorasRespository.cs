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

        public Task<List<JornaleroConHoras>> GetJornalerosConHorasAsync(DateTime fecha)
        {
            return _db.QueryAsync<JornaleroConHoras>(
            @"SELECT j.IdJornalero, j.IdCuadrilla, j.Nombre,
                 IFNULL(h.HN, 0) AS HN,
                 IFNULL(h.HE1, 0) AS HE1,
                 IFNULL(h.HE2, 0) AS HE2,
                 CASE WHEN h.Id IS NULL THEN 1 ELSE 0 END AS Falta
          FROM Jornalero j
          LEFT JOIN Horas h ON j.IdJornalero = h.IdJornalero AND h.Fecha = ?
          ORDER BY j.Nombre", fecha);
        }
    }
}