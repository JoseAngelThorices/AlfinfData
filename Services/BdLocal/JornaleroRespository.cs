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
                var idsOdoo = jornaleros.Select(j => j.IdJornalero).ToList();

                foreach (var j in jornaleros)
                {
                    conn.InsertOrReplace(j);
                }

                var idsLocales = conn.Table<Jornalero>().Select(j => j.IdJornalero).ToList();
                var idsABorrar = idsLocales.Except(idsOdoo).ToList();

                foreach (var id in idsABorrar)
                {
                    conn.Execute("DELETE FROM Jornalero WHERE IdJornalero = ?", id);
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

            return _db.ExecuteAsync(sql, isActive, idJornalero);
        } 


        public async Task<Jornalero?> GetByIdAsync(int id)
        {
            return await _db.Table<Jornalero>()
                .Where(j => j.IdJornalero == id)
                .FirstOrDefaultAsync();
        }

        public Task<Jornalero> GetJornaleroBySerialAsync(string serial)
        {
            return _db
                .Table<Jornalero>()
                .Where(j => j.TarjetaNFC == serial)
                .FirstOrDefaultAsync();
        }

        public Task<List<Jornalero>> GetAllAsync()
            => _db.Table<Jornalero>().ToListAsync();

        public Task UpdateAsync(Jornalero jornalero)
        {
            return _db.UpdateAsync(jornalero);
        }

        public Task<List<Jornalero>> GetJornalerosActivosPorCuadrillaAsync(int idCuadrilla)
        {
            return _db.Table<Jornalero>()
                .Where(j => j.IdCuadrilla == idCuadrilla && j.Activo == true)
                .ToListAsync();
        }

        public Task<List<Jornalero>> GetJornalerosActivosAsync()
        {
            return _db.Table<Jornalero>()
                      .Where(j => j.Activo == true)
                      .ToListAsync();
        }

        public Task UpdateManyAsync(IEnumerable<Jornalero> jornaleros)
        {
            return _db.UpdateAllAsync(jornaleros.ToList());
        }

        public async Task<List<Cuadrilla>> GetCuadrillasConJornalerosAsync()
        {
            const string sql = @"
                SELECT DISTINCT c.IdCuadrilla, c.Descripcion
                FROM Cuadrilla c
                INNER JOIN Jornalero j ON j.IdCuadrilla = c.IdCuadrilla
                WHERE j.Activo = 1
                ORDER BY c.Descripcion;";

            return await _db.QueryAsync<Cuadrilla>(sql);
        }


    }
}