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

                foreach (var c in cuadrillas)
                {
                    // ¿Ya existe uno con este IdOdoo?
                    var exist = conn
                       .Table<Cuadrilla>()
                       .FirstOrDefault(x => x.IdOdoo == c.IdOdoo);

                    if (exist != null)
                    {
                        // Si existe, reutilizamos su IdJornalero y hacemos UPDATE
                        c.IdCuadrilla = exist.IdCuadrilla;
                        conn.Update(c);
                    }
                    else
                    {
                        // Si no existe, dejamos IdJornalero = 0 y hacemos INSERT
                        conn.Insert(c);
                    }
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
