using AlfinfData.Models.Odoo;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace AlfinfData.Services.odoo
{

    
    public interface ITarjetaNFCServices
    {
        Task CreateTarjetasNFCAsync(IEnumerable<TarjetaNFC> tarjetas);
    }
    public class TarjetaNFCServices : ITarjetaNFCServices
    {

        private readonly OdooService _odoo;
        private readonly ILogger<TarjetaNFCServices> _logger;
        public async Task CreateTarjetasNFCAsync(IEnumerable<TarjetaNFC> tarjetas)
        {
            try
            {
                var listaTarjetas = tarjetas
                .Select(t => new {
                    number_id = t.IdTarjetaNFC,
                    nfc_code = t.NumeroSerie
                }).ToList();

            // Opción A: Console.WriteLine
            //foreach (var t in lista)
            //{
            //    Console.WriteLine($"Tarjeta: Id={t.Id}, Nombre={t.IdTarjetaNFC}, Código={t.NumeroSerie}");
            //}

            await _odoo.CallAsync(
                model: "sm.nfc_card",
                method: "create",
                args: new object[] { listaTarjetas },
                kwargs: new { }
            );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarjetas NFC: {Message}", ex.Message);
                throw;
            }

        }

        public TarjetaNFCServices(
            OdooService odoo,
            ILogger<TarjetaNFCServices> logger)
        {
            _odoo = odoo ?? throw new ArgumentNullException(nameof(odoo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


    }
}
