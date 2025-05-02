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

        //METODO PARA LA INSERCCION DE DATOS 
        // Recibe una lista de jornaleros y los inserta o lo reemplaza en la base de datos
        // Si en IdOdoo ya existe, reemplaza el registro existente
        // Si no existe, inserta un nuevo registro
        public Task UpsertJornalerosAsync(IEnumerable<Jornalero> jornaleros) 
        {

            return _db.RunInTransactionAsync(conn =>
            {
                
                foreach (var j in jornaleros)
                {
                   conn.InsertOrReplace(j);
                }
            });

        }

        //Obtiene todos los registros que esten en la tabla jornalero
        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();

      
    }
}
