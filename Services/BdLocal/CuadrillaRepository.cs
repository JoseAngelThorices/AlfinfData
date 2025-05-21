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
                // 1. Obtener IDs que vienen de Odoo
                var idsOdoo = cuadrillas.Select(c => c.IdCuadrilla).ToList();

                // 2. Insertar o actualizar
                foreach (var c in cuadrillas)
                {
                    conn.InsertOrReplace(c);
                }

                // 3. Eliminar las cuadrillas que no están en Odoo
                var idsLocales = conn.Table<Cuadrilla>().Select(c => c.IdCuadrilla).ToList();
                var idsABorrar = idsLocales.Except(idsOdoo).ToList();

                foreach (var id in idsABorrar)
                {
                    conn.Execute("DELETE FROM Cuadrilla WHERE IdCuadrilla = ?", id);
                }
            });
        }


        public Task<List<Cuadrilla>> GetAllAsync()
            => _db.Table<Cuadrilla>().ToListAsync();

        
    }
}
