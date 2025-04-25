using AlfinfData.Models.SQLITE;
using SQLite;

namespace AlfinfData.Services.BdLocal
{
    public class JornaleroRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public JornaleroRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }

        public Task UpsertJornalerosAsync(IEnumerable<Jornalero> jornaleros)
        {
            
            return _db.RunInTransactionAsync(conn =>
            {
                foreach (var j in jornaleros)
                    conn.InsertOrReplace(j);
            });
        }

        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();

        // Otros métodos específicos de Empleado...
    }
}
