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
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly FichajeRepository _fichajeRepo;

        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        [ObservableProperty]
        private List<JornaleroConHoras> seleccionados = new();

        public ObservableCollection<JornaleroConHoras> JornalerosConHoras { get; set; } = new();
        private List<JornaleroConHoras> todosLosJornaleros { get; set; } = new();

        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        public HorasViewModel(
                HorasRepository horasRepo,
                CuadrillaRepository cuadrillaRepo,
                JornaleroRepository jornaleroRepo,
                FichajeRepository fichajeRepo)
        {
            _horasRepo = horasRepo;
            _cuadrillaRepo = cuadrillaRepo;
            _jornaleroRepo = jornaleroRepo;
            _fichajeRepo = fichajeRepo;
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




        public async Task CargarDesdeActivosAsync()
        {
            var activos = await _jornaleroRepo.GetAllAsync();
            var soloActivos = activos.Where(j => j.Activo == true).ToList();

            // Obtenemos todos los fichajes reales de hoy
            var fichajesHoy = await _fichajeRepo.GetJornaleroEntradasAsync();  // Este método ya te devuelve los fichajes de tipo "Entrada" de hoy

            todosLosJornaleros.Clear();

            foreach (var j in soloActivos)
            {
                // Buscar su fichaje de hoy
                var entrada = fichajesHoy.FirstOrDefault(f => f.IdJornalero == j.IdJornalero);

                if (entrada == null || entrada.HoraEficaz == default)
                    continue;  // Si no tiene fichaje, lo ignoramos

                var horaInicio = entrada.HoraEficaz;
                var totalHoras = (DateTime.Now - horaInicio).TotalHours;

                System.Diagnostics.Debug.WriteLine($"Jornalero {j.Nombre}: Total = {totalHoras}");

                var hn = Math.Min(totalHoras, 6.5);
                var he1 = Math.Max(0, totalHoras - 6.5);

                todosLosJornaleros.Add(new JornaleroConHoras
                {
                    IdJornalero = j.IdJornalero,
                    Nombre = j.Nombre,
                    IdCuadrilla = j.IdCuadrilla,
                    HoraActivacion = horaInicio,
                    Hn = Math.Round(hn, 2),
                    He1 = Math.Round(he1, 2),
                    He2 = 0
                });
            }

            FiltrarJornaleros();
        }





    }



}

