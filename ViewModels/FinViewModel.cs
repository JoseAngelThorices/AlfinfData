using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.Input;

public partial class FinViewModel : ObservableObject
{
    private readonly HorasRepository _horasRepo;
    private readonly ProduccionRepository _produccionRepo;

    [ObservableProperty]
    private DateTime fechaDesde = DateTime.Today;

    [ObservableProperty]
    private DateTime fechaHasta = DateTime.Today;

    [ObservableProperty]
    private string resultadoTexto;

    public ObservableCollection<RegistroHistorico> Historico { get; } = new();

    public FinViewModel(HorasRepository horasRepo, ProduccionRepository produccionRepo)
    {
        _horasRepo = horasRepo;
        _produccionRepo = produccionRepo;
    }

    [RelayCommand]
    public async Task GenerarHistoricoAsync()
    {
        Historico.Clear();
        var sb = new StringBuilder();

        for (var dia = FechaDesde.Date; dia <= FechaHasta.Date; dia = dia.AddDays(1))
        {
            var horasDia = await _horasRepo.GetJornalerosConHorasAsync(dia);
            var cajasDia = await _produccionRepo.GetJornalerosConCajasAsync();

            foreach (var j in horasDia)
            {
                var cajas = cajasDia.FirstOrDefault(c => c.IdJornalero == j.IdJornalero)?.TotalCajas ?? 0;

                var registro = new RegistroHistorico
                {
                    NombreJornalero = j.Nombre,
                    Fecha = dia,
                    HN = j.Hn,
                    HE1 = j.He1,
                    HE2 = j.He2,
                    Cajas = cajas
                };

                Historico.Add(registro);

                sb.AppendLine($"🧑 {registro.NombreJornalero}");
                sb.AppendLine($"📅 Fecha: {registro.Fecha:dd/MM/yyyy}");
                sb.AppendLine($"📦 Cajas: {registro.Cajas}");
                sb.AppendLine($"⏱ HN: {registro.HN}, HE1: {registro.HE1}, HE2: {registro.HE2}");
                sb.AppendLine(new string('-', 30));
            }
        }

        ResultadoTexto = Historico.Any() ? sb.ToString() : "No hay registros en ese rango.";
    }
}