using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace AlfinfData.ViewModels
{
    public partial class FinViewModel : ObservableObject
    {
        private readonly FichajeRepository _fichajeRepo;
        private readonly ProduccionRepository _produccionRepo;
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly HistoricoRepository _historicoRepo;

        [ObservableProperty]
        private DateTime fechaDesde = DateTime.Today;

        [ObservableProperty]
        private DateTime fechaHasta = DateTime.Today;

        [ObservableProperty]
        private string resultadoTexto = string.Empty;

        public ObservableCollection<RegistroHistorico> Historico { get; } = new();

        public FinViewModel(
            FichajeRepository fichajeRepo,
            ProduccionRepository produccionRepo,
            JornaleroRepository jornaleroRepo,
            HistoricoRepository historicoRepo)

        {
            _fichajeRepo = fichajeRepo;
            _produccionRepo = produccionRepo;
            _jornaleroRepo = jornaleroRepo;
            _historicoRepo = historicoRepo;
        }


        private string FormatearHorasComoTexto(double horasDecimales)
        {
            var ts = TimeSpan.FromHours(horasDecimales);
            return $"{(int)ts.TotalHours}:{ts.Minutes:D2}";
        }


        [RelayCommand]
        public async Task VisualizarHistoricoAsync()
        {
            Historico.Clear();
            var sb = new StringBuilder();

            var registros = await _historicoRepo.GetAllAsync();
            var filtrados = registros
                .Where(r => r.Fecha.Date >= FechaDesde.Date && r.Fecha.Date <= FechaHasta.Date)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.NombreJornalero)
                .ToList();

            foreach (var r in filtrados)
            {
                Historico.Add(r);

                sb.AppendLine($"🧑 {r.NombreJornalero}");
                sb.AppendLine($"📅 Fecha: {r.Fecha:dd/MM/yyyy}");
                sb.AppendLine($"📦 Cajas: {r.Cajas}");
                sb.AppendLine($"⏱ HN: {FormatearHorasComoTexto(r.HN)}, HE1: {FormatearHorasComoTexto(r.HE1)}");
                sb.AppendLine(new string('-', 30));
            }

            ResultadoTexto = filtrados.Any() ? sb.ToString() : "No hay registros en ese rango.";
        }



        [RelayCommand]
        public async Task GenerarHistoricoAsync()
        {
            Historico.Clear();

            var hoy = DateTime.Today;

            // 1. Obtener todos los fichajes de hoy
            var fichajesDelDia = await _fichajeRepo.GetFichajesSalidasAsync();
            var fichajesHoy = fichajesDelDia

                .Where(f => f.TipoFichaje == "Salida")
                .GroupBy(f => f.IdJornalero)
                .ToList();

            var cajasHoy = await _produccionRepo.GetJornalerosConCajasAsync();

            bool seGeneroAlMenosUnRegistro = false;

            foreach (var grupo in fichajesHoy)
            {
                int? idJornalero = grupo.Key;
                if (!idJornalero.HasValue)
                    continue;

                var jornalero = await _jornaleroRepo.GetByIdAsync(idJornalero.Value);
                if (jornalero == null)
                    continue;

                var horasTotales = await _fichajeRepo.CalcularHorasTrabajadasAsync(idJornalero.Value, hoy);
                var hn = Math.Min(horasTotales, 6.5);
                var he1 = Math.Max(0, horasTotales - 6.5);

                var cajas = cajasHoy.FirstOrDefault(c => c.IdJornalero == idJornalero.Value)?.TotalCajas ?? 0;

                var registro = new RegistroHistorico
                {
                    NombreJornalero = jornalero.Nombre ?? "Sin nombre",
                    Fecha = hoy,
                    HN = Math.Round(hn, 2),
                    HE1 = Math.Round(he1, 2),
                    HE2 = 0,
                    Cajas = cajas
                };

                await _historicoRepo.InsertOrUpdateAsync(registro);
                seGeneroAlMenosUnRegistro = true;
            }

            ResultadoTexto = string.Empty;

            if (seGeneroAlMenosUnRegistro)
            {
                await Shell.Current.DisplayAlert("Éxito", "Histórico generado correctamente para hoy.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Sin datos", "No hay datos para generar el histórico de hoy.", "OK");
            }
        }
    }
}
   