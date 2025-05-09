

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
        public Task CrearFichajesAsync(Fichaje fichaje)
        {
            return _db.RunInTransactionAsync(conn =>
            {
                    conn.InsertOrReplace(fichaje);   
             
            });
        }

        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();
    }
}
