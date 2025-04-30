using System.Collections.ObjectModel;
using AlfinfData.Services.odoo;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Models.Odoo;
using System.Diagnostics;


namespace AlfinfData.ViewModels
{


    public partial class DescargasViewModel : ObservableObject
    {
        private readonly IEmpleadosService _empleadosService;     // servicio Odoo Empleado
        private readonly ICuadrillasService _cuadrillaService;    // servicio Odoo Cuadrilla
        private readonly JornaleroRepository _jornaleroRepo;      // repositorio SQLite
        private readonly CuadrillaRepository _cuadrillaRepo;
        public DescargasViewModel(IEmpleadosService empleadosService, ICuadrillasService cuadrillaService, JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _empleadosService = empleadosService;
            _cuadrillaService = cuadrillaService;
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
            Empleados = new ObservableCollection<Empleado>();
            Cuadrillas = new ObservableCollection<CuadrillaOdoo>();
        }

        public ObservableCollection<Empleado> Empleados { get; }
        public ObservableCollection<CuadrillaOdoo> Cuadrillas { get; }


        [ObservableProperty]
        private bool isBusy;
        [RelayCommand]
        private async void Entrada()
        {
            await CargarEmpleadosAsync();
        }
        private async Task CargarEmpleadosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                isBusy = true;

                var listaDesdeOdoo = await _empleadosService.GetAllAsync();
                var listaLocal = listaDesdeOdoo.Select(o => new Jornalero
                {
                    Nombre = o.Nombre,
                    IdCuadrilla = o.Id_Departamento,   
                    IdJornalero = o.Id,
                    Activo = o.Activo
                }).ToList(); // ahora es List<Jornalero>
                //Para ver los datos que se estan pasando por la terminal de salida
                //foreach (var j in listaLocal)
                //{
                //    Debug.WriteLine(
                //        $"[listaLocal] IdOdoo={j.IdOdoo}, Nombre=\"{j.Nombre}\", IdCuadrilla={j.IdCuadrilla}, TarjetaNFC={j.TarjetaNFC}"
                //    );
                //}
                await _jornaleroRepo.UpsertJornalerosAsync(listaLocal);
                
                await Shell.Current.DisplayAlert("Success", "Se han bajado con exito los datos!", "OK");
            }
            catch (Exception ex)
            {
                // Mostrar alerta de error, log, etc.
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                isBusy = false;
            }
        }
        [RelayCommand]
        private async void Cuadrilla()
        {
            await CargarCuadrillaAsync();
        }
        private async Task CargarCuadrillaAsync()
        {
            if (IsBusy)
                return;

            try
            {
                isBusy = true;

                var listaDesdeOdoo = await _cuadrillaService.GetAllAsync();
                var listaLocal = listaDesdeOdoo.Select(o => new Cuadrilla
                {
                    Descripcion = o.Descripcion,
                    IdCuadrilla = o.IdCuadrilla
                }).ToList(); // ahora es List<Cuadrilla>
                //Para ver los datos que se estan pasando por la terminal de salida
                //foreach (var j in listaLocal)
                //{
                //   Debug.WriteLine(
                //        $"[listaLocal] IdOdoo={j.IdOdoo}, Nombre=\"{j.Descripcion}\""
                //    );
                //}
                await _cuadrillaRepo.UpsertCuadrillaAsync(listaLocal);

                await Shell.Current.DisplayAlert("Success", "Se han bajado con exito los datos!", "OK");
            }
            catch (Exception ex)
            {
                // Mostrar alerta de error, log, etc.
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                isBusy = false;
            }
        }



    }
}
