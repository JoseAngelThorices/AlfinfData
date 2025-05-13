

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
            var existente = await _db.FindAsync<Fichaje>(fichaje.IdJornalero);
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
        public void BorrarDatosAsync()
        {
            _db.DeleteAllAsync<Produccion>();
            _db.DeleteAllAsync<Horas>();
        }
            

        public Task ActualizarHoraEficazAsync(int id, DateTime nuevaHora)
        {
            return _db.RunInTransactionAsync(conn =>
            {
                // El placeholder ? se sustituye por los parámetros en orden
                conn.Execute(
                  "UPDATE Fichaje SET HoraEficaz = ? WHERE IdJornalero = ?",
                  nuevaHora,
                  id
                );
            });
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
        _db.QueryAsync<JornaleroEntrada>(
        @"SELECT 
            f.IdJornalero,
            j.Nombre,
            f.HoraEficaz
          FROM Fichaje AS f
          INNER JOIN Jornalero AS j
            ON f.IdJornalero = j.IdJornalero;"
        );
        public Task<List<Fichaje>> GetAllAsync()
            => _db.Table<Fichaje>().ToListAsync();
    }
}
