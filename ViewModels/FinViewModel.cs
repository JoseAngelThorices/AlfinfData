using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

public partial class FinViewModel : ObservableObject
{
    private readonly FichajeRepository _fichajeRepo;
    private readonly ProduccionRepository _produccionRepo;
    private readonly JornaleroRepository _jornaleroRepo;

    [ObservableProperty]
    private DateTime fechaDesde = DateTime.Today;

    [ObservableProperty]
    private DateTime fechaHasta = DateTime.Today;

    [ObservableProperty]
    private string resultadoTexto;

    public ObservableCollection<RegistroHistorico> Historico { get; } = new();

    public FinViewModel(
        FichajeRepository fichajeRepo,
        ProduccionRepository produccionRepo,
        JornaleroRepository jornaleroRepo)
    {
        _fichajeRepo = fichajeRepo;
        _produccionRepo = produccionRepo;
        _jornaleroRepo = jornaleroRepo;
    }

    private string FormatearHorasComoTexto(double horasDecimales)
    {
        var ts = TimeSpan.FromHours(horasDecimales);
        return $"{(int)ts.TotalHours}:{ts.Minutes:D2}";
    }


    [RelayCommand]
    public async Task GenerarHistoricoAsync()
    {
        Historico.Clear();
        var sb = new StringBuilder();

        for (var dia = FechaDesde.Date; dia <= FechaHasta.Date; dia = dia.AddDays(1))
        {
            // 1. Obtener todos los fichajes de ese día
            var fichajesDelDia = await _fichajeRepo.GetAllAsync();
            var fichajesDiaFiltrados = fichajesDelDia
                .Where(f => f.HoraEficaz.Date == dia.Date)
                .GroupBy(f => f.IdJornalero)
                .ToList();

            var cajasDia = await _produccionRepo.GetJornalerosConCajasAsync();

            foreach (var grupo in fichajesDiaFiltrados)
            {
                int? idJornalero = grupo.Key;

                if (!idJornalero.HasValue)
                    continue;

                var jornalero = await _jornaleroRepo.GetByIdAsync(idJornalero.Value);
                if (jornalero == null)
                    continue;

                var horasTotales = await _fichajeRepo.CalcularHorasTrabajadasAsync(idJornalero.Value, dia);
                var hn = Math.Min(horasTotales, 6.5);
                var he1 = Math.Max(0, horasTotales - 6.5);

                var cajas = cajasDia.FirstOrDefault(c => c.IdJornalero == idJornalero.Value)?.TotalCajas ?? 0;

                var registro = new RegistroHistorico
                {
                    NombreJornalero = jornalero.Nombre ?? "Sin nombre",
                    Fecha = dia,
                    HN = Math.Round(hn, 2),
                    HE1 = Math.Round(he1, 2),
                    HE2 = 0,
                    Cajas = cajas
                };

                Historico.Add(registro);

                sb.AppendLine($"🧑 {registro.NombreJornalero}");
                sb.AppendLine($"📅 Fecha: {registro.Fecha:dd/MM/yyyy}");
                sb.AppendLine($"📦 Cajas: {registro.Cajas}");
                sb.AppendLine($"⏱ HN: {FormatearHorasComoTexto(registro.HN)}, " +
                              $"HE1: {FormatearHorasComoTexto(registro.HE1)}, " +
                              $"HE2: {FormatearHorasComoTexto(registro.HE2)}");
                sb.AppendLine(new string('-', 30));
            }
        }

        ResultadoTexto = Historico.Any() ? sb.ToString() : "No hay registros en ese rango.";
    }
}