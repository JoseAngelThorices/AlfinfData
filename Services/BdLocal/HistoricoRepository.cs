using SQLite;
using AlfinfData.Models.SQLITE;

namespace AlfinfData.Services.BdLocal
{
    public class HistoricoRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public HistoricoRepository(DatabaseService databaseService)
        {
            _db = databaseService.Conn;
        }

        public Task<List<RegistroHistorico>> GetAllAsync()
        {
            return _db.Table<RegistroHistorico>().ToListAsync();
        }

        public async Task InsertOrUpdateAsync(RegistroHistorico registro)
        {
            var existente = await _db.Table<RegistroHistorico>()
                .Where(r => r.NombreJornalero == registro.NombreJornalero && r.Fecha == registro.Fecha)
                .FirstOrDefaultAsync();

            if (existente != null)
            {
                registro.Id = existente.Id;
                await _db.UpdateAsync(registro);
            }
            else
            {
                await _db.InsertAsync(registro);
            }
        }

    }
}
