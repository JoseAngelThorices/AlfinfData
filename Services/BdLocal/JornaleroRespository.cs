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


        //Obtiene todos los registros que esten en la tabla jornalero
        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();


        // Actualiza un solo jornalero
        public Task UpdateAsync(Jornalero jornalero)
        {
            return _db.UpdateAsync(jornalero);
        }

        // Actualiza varios jornaleros a la vez
        public Task UpdateManyAsync(IEnumerable<Jornalero> jornaleros)
        {
            return _db.UpdateAllAsync(jornaleros.ToList());
        }

    }
}
