using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AlfinfData.Services;
using AlfinfData.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;


namespace AlfinfData.ViewModels
{
    public class EmpleadosViewModel : INotifyPropertyChanged
    {
        private readonly OdooService _odoo;

        public ObservableCollection<Empleado> Empleados { get; }
            = new ObservableCollection<Empleado>();

        public EmpleadosViewModel(OdooService odoo) => _odoo = odoo;

        public async Task LoadEmpleadosAsync()
        {
            var datos = await _odoo.GetEmpleadosAsync();
            Empleados.Clear();
            foreach (var e in datos)
                Empleados.Add(e);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Implementa INotifyPropertyChanged…
    }
}
