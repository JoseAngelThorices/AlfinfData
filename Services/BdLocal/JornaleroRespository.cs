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
                    // ¿Ya existe uno con este IdOdoo?
                    var exist = conn
                       .Table<Jornalero>()
                       .FirstOrDefault(x => x.IdOdoo == j.IdOdoo);

                    if (exist != null)
                    {
                        // Si existe, reutilizamos su IdJornalero y hacemos UPDATE
                        j.IdJornalero = exist.IdJornalero;
                        conn.Update(j);
                    }
                    else
                    {
                        // Si no existe, dejamos IdJornalero = 0 y hacemos INSERT
                        conn.Insert(j);
                    }
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
