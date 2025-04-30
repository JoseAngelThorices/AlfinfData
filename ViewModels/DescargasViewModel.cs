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
        private readonly IEmpleadosService _empleadosService;    // servicio Odoo
        private readonly JornaleroRepository _jornaleroRepo;      // repositorio SQLite
        public DescargasViewModel(IEmpleadosService empleadosService, JornaleroRepository jornaleroRepo)
        {
            _empleadosService = empleadosService;
            _jornaleroRepo = jornaleroRepo;
            Empleados = new ObservableCollection<Empleado>();
        }

        public ObservableCollection<Empleado> Empleados { get; }


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
                    IdOdoo = o.Id,
                    TarjetaNFC = o.TarjetaNFC
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
            await CargarEmpleadosAsync();
        }



    }
}
