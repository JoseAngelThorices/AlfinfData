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
                // 1. Obtener IDs que vienen de Odoo
                var idsOdoo = jornaleros.Select(j => j.IdJornalero).ToList();

                // 2. Insertar o actualizar
                foreach (var j in jornaleros)
                {
                    conn.InsertOrReplace(j);
                }

                // 3. Eliminar los que no están ya en Odoo
                var idsLocales = conn.Table<Jornalero>().Select(j => j.IdJornalero).ToList();
                var idsABorrar = idsLocales.Except(idsOdoo).ToList();

                foreach (var id in idsABorrar)
                {
                    conn.Execute("DELETE FROM Jornalero WHERE IdJornalero  = ?", id);
                }
            });
        }
        public Task<int> SetActiveAsync(int idJornalero, bool isActive)
        {
            const string sql = @"
                UPDATE Jornalero
                SET Activo = ?
                WHERE IdJornalero = ?;
            ";

            // sqlite-net sabe mapear bool a INTEGER (1 o 0)
            return _db.ExecuteAsync(sql, isActive, idJornalero);
        }
        public Task<Jornalero> GetJornaleroBySerialAsync(string serial)
        {
            // Usamos la tabla mapeada y LINQ para filtrar por el campo Serial
            return _db
                .Table<Jornalero>()
                .Where(j => j.TarjetaNFC == serial)
                .FirstOrDefaultAsync();
        }


        //Obtiene todos los registros que esten en la tabla jornalero
        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();

      
    }
}
