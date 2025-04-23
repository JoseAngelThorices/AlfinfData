using SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using AlfinfData.Models;
using System.Diagnostics;
using YourMauiApp.Models;

namespace AlfinfData.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        public async Task InitAsync()
        {
            if (_db != null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");
            _db = new SQLiteAsyncConnection(dbPath);

            await _db.ExecuteAsync("PRAGMA foreign_keys = ON;");

            await _db.CreateTableAsync<cuadrilla>();
            await _db.CreateTableAsync<jornalero>();
            await _db.CreateTableAsync<traza>();
            await _db.CreateTableAsync<formato>();
            await _db.CreateTableAsync<horas>();
            await _db.CreateTableAsync<produccion>();
            await _db.CreateTableAsync<fichaje>();
        }

        public Task<List<cuadrilla>> GetCuadrillasAsync()
        {
            return _db.Table<cuadrilla>().ToListAsync();
        }

        public async Task InsertarCuadrillasDePruebaAsync()
        {
            var existentes = await _db.Table<cuadrilla>().ToListAsync();
            if (existentes.Count == 0)
            {
                var lista = new List<cuadrilla>
                {
                    new cuadrilla { Descripcion = "Cuadrilla 1" },
                    new cuadrilla { Descripcion = "Cuadrilla 2" },
                    new cuadrilla { Descripcion = "Cuadrilla 3" }
                };
                await _db.InsertAllAsync(lista);
            }
        }
    }
}
