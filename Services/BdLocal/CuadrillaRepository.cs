using AlfinfData.Models.SQLITE;
using SQLite;

namespace AlfinfData.Services.BdLocal
{

    public class CuadrillaRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public CuadrillaRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }

        public Task UpsertCuadrillaAsync(IEnumerable<Cuadrilla> cuadrillas)
        {

            return _db.RunInTransactionAsync(conn =>
            {             
                foreach (var j in cuadrillas){
                    // InsertOrReplace insertará si no existe,
                    // o actualizará si ya hay un registro con esa PK.
                    conn.InsertOrReplace(j);
                }

                // Para saber cuantos registros hay
                //var todos = conn.Table<Jornalero>().ToList();
                //Debug.WriteLine($"[BD] Total jornaleros tras upsert: {todos.Count}");
            });

        }

        public Task<List<Cuadrilla>> GetAllAsync()
            => _db.Table<Cuadrilla>().ToListAsync();

        // Otros métodos específicos de Empleado...
    }
}
