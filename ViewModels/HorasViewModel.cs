using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class HorasViewModel : ObservableObject
    {
        private readonly HorasRepository _horasRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;

        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        [ObservableProperty]
        private List<JornaleroConHoras> seleccionados = new();

        public ObservableCollection<JornaleroConHoras> JornalerosConHoras { get; set; } = new();
        private List<JornaleroConHoras> todosLosJornaleros { get; set; } = new();

        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        public HorasViewModel(HorasRepository horasRepo, CuadrillaRepository cuadrillaRepo)
        {
            _horasRepo = horasRepo;
            _cuadrillaRepo = cuadrillaRepo;
        }

        public async Task CargarCuadrillasAsync()
        {
            var lista = await _cuadrillaRepo.GetAllAsync();
            Cuadrillas.Clear();
            foreach (var c in lista)
                Cuadrillas.Add(c);
        }

        public async Task GuardarHorasAsync()
        {
            if (JornalerosConHoras == null || !JornalerosConHoras.Any())
                return;

            foreach (var j in JornalerosConHoras)
            {
                var horas = new Horas
                {
                    IdJornalero = j.IdJornalero,
                    Fecha = FechaSeleccionada, // este campo ya lo tienes
                    HN = j.Hn,
                    HE1 = j.He1,
                    HE2 = j.He2
                };

                await _horasRepo.InsertarHorasAsync(horas);
            }
        }




        public async Task CargarJornalerosConHorasAsync()
        {
            var lista = await _horasRepo.GetJornalerosConHorasAsync(FechaSeleccionada);
            todosLosJornaleros = lista;
            FiltrarJornaleros();
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            FiltrarJornaleros();
        }

        private void FiltrarJornaleros()
        {
            JornalerosConHoras.Clear();

            var idCuadrilla = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var filtrados = CuadrillaSeleccionada == null
                ? todosLosJornaleros
                : todosLosJornaleros.Where(j => j.IdCuadrilla == idCuadrilla);

            foreach (var j in filtrados)
                JornalerosConHoras.Add(j);
        }
    }
}
