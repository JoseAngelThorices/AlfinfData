using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage; // NUEVO
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class ProduccionViewModel : ObservableObject
    {
        private readonly ProduccionRepository _produccionRepository;
        private readonly CuadrillaRepository _repoC;

        [ObservableProperty]
        private List<JornaleroConCajas> seleccionados = new();

        public ObservableCollection<JornaleroConCajas> JornalerosConCajas { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        private List<JornaleroConCajas> TodosLosJornaleros { get; set; } = new();

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        public ProduccionViewModel(ProduccionRepository produccionRepository, CuadrillaRepository repoC)
        {
            _produccionRepository = produccionRepository;
            _repoC = repoC;
        }

        public async Task CargarCuadrillaAsync()
        {
            var lista = await _repoC.GetAllAsync();
            Cuadrillas.Clear();

            // Agregar opción TODOS
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" });

            foreach (var c in lista)
                Cuadrillas.Add(c);

            // Restaurar cuadrilla previa
            var savedId = Preferences.Default.Get("CuadrillaSeleccionada_Produccion", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == savedId);
        }

        public async Task CargarJornalerosConCajasAsync()
        {
            var jornalerosConCajas = await _produccionRepository.GetJornalerosConCajasAsync();
            TodosLosJornaleros = jornalerosConCajas.ToList();
            FiltrarJornaleros();
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            Preferences.Default.Set("CuadrillaSeleccionada_Produccion", value?.IdCuadrilla ?? 0);
            FiltrarJornaleros();
        }

        private void FiltrarJornaleros()
        {
            JornalerosConCajas.Clear();

            int id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;

            var filtrados = id == 0
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == id);

            foreach (var j in filtrados)
                JornalerosConCajas.Add(j);
        }

        public async Task InsertProduccionAsync(int idJornalero, int cajas)
        {
            var produccion = new Produccion
            {
                IdJornalero = idJornalero,
                Cajas = cajas,
                Timestamp = DateTime.Now
            };

            await _produccionRepository.InsertProduccionAsync(produccion);
        }

        public async Task ProcesarCajasAsync(int cantidad)
        {
            if (Seleccionados is null || Seleccionados.Count == 0)
                return;

            foreach (var j in Seleccionados)
                await InsertProduccionAsync(j.IdJornalero, cantidad);

            await CargarJornalerosConCajasAsync();
        }
    }
}
