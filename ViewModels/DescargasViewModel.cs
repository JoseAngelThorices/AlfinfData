using System.Collections.ObjectModel;
using AlfinfData.Services.odoo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Models;


namespace AlfinfData.ViewModels
{
    

    public partial class DescargasViewModel : ObservableObject
    {
        private readonly IEmpleadosService _empleadosService;
        public DescargasViewModel(IEmpleadosService empleadosService)
        {
            _empleadosService = empleadosService;
            Empleados = new ObservableCollection<Empleado>();
        }
        
        public ObservableCollection<Empleado> Empleados { get; }

       
        [ObservableProperty]
        private bool isBusy;
        private async Task CargarEmpleadosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                isBusy = true;
                
                var lista = await _empleadosService.GetAllAsync();

                // Volcar en la ObservableCollection
                Empleados.Clear();
                foreach (var e in lista)
                    Empleados.Add(e);
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
        private async  void Entrada()
        {
            await CargarEmpleadosAsync();
        }

    }
}
