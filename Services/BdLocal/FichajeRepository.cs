

using AlfinfData.Models.SQLITE;
using SQLite;

namespace AlfinfData.Services.BdLocal
{
    public class FichajeRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public FichajeRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }
        public async Task<bool> CrearFichajesAsync(Fichaje fichaje)
        {
            var existente = await _db.FindAsync<Fichaje>(fichaje.Id);
            if (existente != null)
            {
                // Ya había un registro con ese Id ⇒ no insertamos
                return false;
            }
            await _db.InsertAsync(fichaje);
            return true;
        }
        public Task ActualizarHoraEficazAsync(int id, DateTime nuevaHora)
        {
            return _db.RunInTransactionAsync(conn =>
            {
                // El placeholder ? se sustituye por los parámetros en orden
                conn.Execute(
                  "UPDATE Fichaje SET HoraEficaz = ? WHERE IdJornalero = ?",
                  nuevaHora,
                  id
                );
            });
        }
        public Task<Fichaje> GetFirstByJornaleroAsync(int idJornalero) =>
            _db.Table<Fichaje>()
            .Where(f => f.IdJornalero == idJornalero)
            .FirstOrDefaultAsync();

        public Task<List<JornaleroEntrada>> GetJornaleroEntradasAsync() =>
        _db.QueryAsync<JornaleroEntrada>(
        @"SELECT 
            f.IdJornalero,
            j.Nombre,
            f.HoraEficaz
          FROM Fichaje AS f
          INNER JOIN Jornalero AS j
            ON f.IdJornalero = j.IdJornalero;"
    );
        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();
    }
}
