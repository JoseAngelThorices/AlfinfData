

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
        public async Task<bool> CrearFichajeNuevoDiaAsync(Fichaje fichaje)
        {
             var existente = await _db.Table<Fichaje>()
                        .Where(f => f.IdJornalero == fichaje.IdJornalero)
                        .FirstOrDefaultAsync();
            if (existente != null)
            {
                // Ya había un registro con ese Id ⇒ no insertamos
                return false;
            }
            await _db.InsertAsync(fichaje);
            return true;
        }
        public async Task<bool> CrearFichajesJornalerosAsync(Fichaje fichaje)
        {
            var inicioHoy = DateTime.Today;               
            var inicioManana = inicioHoy.AddDays(1);
            var ultimoHoy = await _db.Table<Fichaje>()
            .Where(f =>
                f.IdJornalero == fichaje.IdJornalero
                && f.InstanteFichaje >= inicioHoy
                && f.InstanteFichaje < inicioManana)                                    
            .OrderByDescending(f => f.Id)               
            .FirstOrDefaultAsync();

            if(ultimoHoy == null)
            {
                await _db.InsertAsync(fichaje);
                return true;
            }
            if (ultimoHoy.TipoFichaje == fichaje.TipoFichaje)
            {
                //Significará que el último registro que recoge, el tipo de fichaje coincide con el que le estás pasando por lo que ya existirá
                return false;
            }
            await _db.InsertAsync(fichaje);
            return true;
        }
        public async Task<bool> BuscarFichajeNuevoDia()
        {
            // 1) Definimos el inicio y fin de “hoy”
            var inicioHoy = DateTime.Today;               // p.ej. 2025-05-13 00:00:00
            var inicioManana = inicioHoy.AddDays(1);        // p.ej. 2025-05-14 00:00:00

            // 2) Buscamos cualquier fichaje de este jornalero entre [hoy, mañana)
            var existentes = await _db.Table<Fichaje>()
                .Where(x => x.IdJornalero == 999999
                         && x.InstanteFichaje >= inicioHoy
                         && x.InstanteFichaje < inicioManana)
                .ToListAsync();

            // 3) Si hay al menos uno, devolvemos true (ya existe ficha hoy)
            if (existentes.Any())
                return true;

            // 4) Si no existe, devolvemos false (puedes crear el nuevo)
            return false;
        }
        public async Task<bool> BorrarDatosAsync()
        {
            await _db.DeleteAllAsync<Produccion>();
            await _db.DeleteAllAsync<Fichaje>();
            await _db.DeleteAllAsync<Horas>();
            const string sql = @"
                UPDATE Jornalero
                SET Activo = ?;
                ";

            await _db.ExecuteAsync(sql, false);
            return true;
        }
            

        public async Task ActualizarHoraEficazAsync(int id, DateTime nuevaHora)
        {
            var inicioHoy = DateTime.Today;
            var inicioManana = inicioHoy.AddDays(1);

            // 1) Intenta actualizar mediante SQL con parámetros de rango
            var filasAfectadas = await _db.ExecuteAsync(
              @"UPDATE Fichaje
                SET HoraEficaz = ?
                WHERE IdJornalero = ?
                  AND TipoFichaje = 'Entrada'
                  AND InstanteFichaje >= ?
                  AND InstanteFichaje <  ?;",
              nuevaHora,
              id,
              inicioHoy,
              inicioManana
            );
        }
        public async Task<Fichaje> BuscarFichajeNuevoDiaDatos()
        {
            var inicioHoy = DateTime.Today;               
            var inicioManana = inicioHoy.AddDays(1);      
            var lista = await _db.Table<Fichaje>()
                .Where(x => x.IdJornalero == 999999
                         && x.InstanteFichaje >= inicioHoy
                         && x.InstanteFichaje < inicioManana)
                .ToListAsync();
            Fichaje primero = lista.FirstOrDefault();
            return primero;
        }

        public Task<List<JornaleroEntrada>> GetJornaleroEntradasAsync() =>
            _db.QueryAsync<JornaleroEntrada>(@"
                SELECT 
                    f.IdJornalero,
                    j.Nombre,
                    f.HoraEficaz
                FROM Fichaje AS f
                INNER JOIN Jornalero AS j
                ON f.IdJornalero = j.IdJornalero
                WHERE date(
                    (f.HoraEficaz - 621355968000000000) / 10000000, 
                    'unixepoch', 
                    'localtime')= date('now', 'localtime')
                AND f.TipoFichaje = 'Entrada';");
        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();
    }
}
