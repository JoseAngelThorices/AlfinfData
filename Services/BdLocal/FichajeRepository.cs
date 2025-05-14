

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
        public async Task<bool> CrearFichajesAsync(Fichaje fichaje)
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
        public async void BorrarDatosAsync()
        {
            await _db.DeleteAllAsync<Produccion>();
            await _db.DeleteAllAsync<Fichaje>();
            await _db.DeleteAllAsync<Horas>();
            const string sql = @"
                UPDATE Jornalero
                SET Activo = ?;
                ";

            await _db.ExecuteAsync(sql, false);
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
                        'localtime'
                      )
                  = date('now', 'localtime');");
        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();



        ////////Fichaje salida.
        ///
        //Sirve para saber si ese jornalero ya ha fichado la salida hoy.
        public async Task<bool> ExisteFichajeSalidaAsync(int idJornalero)
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);

            var salida = await _db.Table<Fichaje>()
                .Where(f =>
                    f.IdJornalero == idJornalero &&
                    f.TipoFichaje == "Salida" &&
                    f.InstanteFichaje >= hoy &&
                    f.InstanteFichaje < manana)
                .FirstOrDefaultAsync();

            return salida != null;
        }

        //Con esto sabre cuando tiempo ha trabajado el jornalero 
        public async Task<Fichaje?> ObtenerEntradaPorJornaleroAsync(int idJornalero)
        {
            return await _db.Table<Fichaje>()
                .Where(f => f.IdJornalero == idJornalero && f.TipoFichaje == "Entrada")
                .OrderByDescending(f => f.HoraEficaz)
                .FirstOrDefaultAsync();
        }
    }
}
