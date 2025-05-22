

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

        //Verificamos si el jornalero esta en entrada o salida.
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

        public async Task<List<JornaleroEntrada>> GetJornaleroEntradasAsync()
        {
            var query = @"
        SELECT f.IdJornalero, j.Nombre, f.HoraEficaz
        FROM Fichaje f
        JOIN Jornalero j ON j.IdJornalero = f.IdJornalero
        WHERE f.Id IN (
            SELECT MAX(f2.Id)
            FROM Fichaje f2
            WHERE date((f2.HoraEficaz - 621355968000000000) / 10000000, 'unixepoch', 'localtime') = date('now', 'localtime')
            GROUP BY f2.IdJornalero
        )
        AND f.TipoFichaje = 'Entrada';
    ";

            return await _db.QueryAsync<JornaleroEntrada>(query);
        }
            public Task<List<JornaleroEntrada>> GetJornaleroSalidasAsync() =>
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
            AND f.TipoFichaje = 'Salida';");


        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();
        public Task<List<Fichaje>> GetFichajesSalidasAsync() =>
         _db.QueryAsync<Fichaje>(@"
        WITH Ultimos AS (
            SELECT
                f.*,
                ROW_NUMBER() OVER (
                    PARTITION BY f.IdJornalero
                    ORDER BY f.HoraEficaz DESC
                ) AS rn
            FROM Fichaje AS f
            WHERE
                date(
                    (f.HoraEficaz - 621355968000000000) / 10000000,
                    'unixepoch',
                    'localtime'
                ) = date('now', 'localtime')
                AND f.TipoFichaje = 'Salida'
        )
        SELECT
            u.*
        FROM Ultimos AS u
        WHERE u.rn = 1;
    ");

        public async Task<List<Fichaje>> GetFichajesOrdenadosPorJornaleroYFechaAsync(int idJornalero, DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            return await _db.Table<Fichaje>()
                .Where(f => f.IdJornalero == idJornalero &&
                            f.HoraEficaz >= inicio &&
                            f.HoraEficaz < fin)
                .OrderBy(f => f.HoraEficaz)
                .ToListAsync();
        }



        public async Task<double> CalcularHorasTrabajadasAsync(int idJornalero, DateTime fecha)
        {
            var fichajes = await GetFichajesOrdenadosPorJornaleroYFechaAsync(idJornalero, fecha);

            double totalHoras = 0;
            Fichaje? entradaPendiente = null;

            foreach (var f in fichajes)
            {
                if (f.TipoFichaje == "Entrada")
                {
                    // Guarda la entrada pendiente si no hay otra en espera
                    if (entradaPendiente == null)
                        entradaPendiente = f;
                }
                else if (f.TipoFichaje == "Salida" && entradaPendiente != null)
                {
                    // Si hay una entrada pendiente y aparece una salida, calcula el intervalo
                    var horas = (f.HoraEficaz - entradaPendiente.HoraEficaz).TotalHours;

                    // Si la hora es válida, la añadimos al total
                    if (horas > 0)
                        totalHoras += horas;

                    // Reinicia la entrada pendiente
                    entradaPendiente = null;
                }
            }

            return totalHoras;
        }




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

        public async Task<List<Fichaje>> GetUltimosFichajesDelDiaAsync()
        {
            var inicioHoy = DateTime.Today;
            var finHoy = inicioHoy.AddDays(1);

            // Usamos HoraEficaz porque es de tipo DateTime y se puede filtrar en SQLite
            var fichajesHoy = await _db.Table<Fichaje>()
                .Where(f => f.HoraEficaz >= inicioHoy && f.HoraEficaz < finHoy)
                .ToListAsync();

            // Luego agrupamos por jornalero y nos quedamos con el último por InstanteFichaje
            var ultimos = fichajesHoy
                .GroupBy(f => f.IdJornalero)
                .Select(g => g.OrderByDescending(f => f.InstanteFichaje).First())
                .ToList();

            return ultimos;
        }



    }
}
