using SQLite;
using AlfinfData.Models.SQLITE;

namespace AlfinfData.Services.BdLocal
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath) // Le pasamos url (mauiprogram)
        {
            // Crear la conexión
            _db = new SQLiteAsyncConnection(dbPath);

            // Crear las tablas
            _db.CreateTableAsync<Cuadrilla>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Jornalero>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Traza>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Formato>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Horas>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Produccion>().GetAwaiter().GetResult();
            _db.CreateTableAsync<Fichaje>().GetAwaiter().GetResult();
        }
        //conexion con la base de datos
        public SQLiteAsyncConnection Conn => _db;
    }
}
