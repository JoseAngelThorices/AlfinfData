using System.Diagnostics;
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
                {
                    // InsertOrReplace insertará si no existe,
                    // o actualizará si ya hay un registro con esa PK.
                    conn.InsertOrReplace(j);
                }

                // Para saber cuantos registros hay
                //var todos = conn.Table<Jornalero>().ToList();
                //Debug.WriteLine($"[BD] Total jornaleros tras upsert: {todos.Count}");
            });

        }

        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();

        // Otros métodos específicos de Empleado...
    }
}
