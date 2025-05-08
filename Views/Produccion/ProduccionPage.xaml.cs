using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AlfinfData.Views.Produccion
{
    public partial class ProduccionPage : ContentPage
    {
        public ObservableCollection<Jornalero> Jornaleros { get; set; }

        public ProduccionPage()
        {
            InitializeComponent();

            // Cargar datos desde Preferences
            Jornaleros = new ObservableCollection<Jornalero>
            {
                new Jornalero { IdJornalero = 1, Nombre = "Juan Pérez", Cajas = Preferences.Get("Cajas_1", 0) },
                new Jornalero { IdJornalero = 2, Nombre = "María García", Cajas = Preferences.Get("Cajas_2", 0) },
                new Jornalero { IdJornalero = 3, Nombre = "Carlos López", Cajas = Preferences.Get("Cajas_3", 0) },
                new Jornalero { IdJornalero = 4, Nombre = "Ana Martínez", Cajas = Preferences.Get("Cajas_4", 0) },
                new Jornalero { IdJornalero = 5, Nombre = "Carlos Sánchez", Cajas = Preferences.Get("Cajas_5", 0) }
            };

            ListaDeJornaleros.ItemsSource = Jornaleros;

            // Asignar manejadores a botones 1-5 y N
            Btn1.Clicked += (s, e) => OnCantidadFijaClicked(1);
            Btn2.Clicked += (s, e) => OnCantidadFijaClicked(2);
            Btn3.Clicked += (s, e) => OnCantidadFijaClicked(3);
            Btn4.Clicked += (s, e) => OnCantidadFijaClicked(4);
            Btn5.Clicked += (s, e) => OnCantidadFijaClicked(5);
            BtnN.Clicked += OnBtnNClicked;
        }

        // Botones 1 al 5: elige sumar o restar, luego confirma antes de aplicar
        private async void OnCantidadFijaClicked(int cantidad)
        {
            string accion = await DisplayActionSheet(
                $"¿Qué deseas hacer con {cantidad} {Pluralizar("caja", cantidad)}?",
                "Cancelar", null, "Añadir cajas", "Restar cajas");

            if (accion == "Cancelar" || string.IsNullOrWhiteSpace(accion))
                return;

            bool esSuma = accion == "Añadir cajas";
            int cajasFinal = esSuma ? cantidad : -cantidad;

            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<Jornalero>().ToList();
            if (!seleccionados.Any())
            {
                await DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
                return;
            }

            string resumen = CrearResumenAccion(cajasFinal, seleccionados.Count);
            bool confirmar = await DisplayAlert("Confirmar acción", resumen, "OK", "Cancelar");

            if (confirmar)
                AddBoxes(cajasFinal);
        }

        // Botón N: permite escribir el número, luego elige sumar o restar y confirmas
        private async void OnBtnNClicked(object sender, EventArgs e)
        {
            string accion = await DisplayActionSheet(
                "¿Qué deseas hacer?", "Cancelar", null, "Añadir cajas", "Restar cajas");

            if (accion == "Cancelar" || string.IsNullOrWhiteSpace(accion))
                return;

            bool esSuma = accion == "Añadir cajas";
            string promptTitle = esSuma ? "Número de cajas a añadir" : "Número de cajas a restar";

            string result = await DisplayPromptAsync(
                promptTitle, "Introduce el número de cajas:", "Aceptar", "Cancelar",
                "0", maxLength: 3, keyboard: Keyboard.Numeric);

            if (!int.TryParse(result, out int cajas) || cajas <= 0)
            {
                if (!string.IsNullOrWhiteSpace(result))
                    await DisplayAlert("Error", "Introduce un número válido mayor que cero.", "OK");
                return;
            }

            int cajasFinal = esSuma ? cajas : -cajas;

            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<Jornalero>().ToList();
            if (!seleccionados.Any())
            {
                await DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
                return;
            }

            string resumen = CrearResumenAccion(cajasFinal, seleccionados.Count);
            bool confirmar = await DisplayAlert("Confirmar acción", resumen, "OK", "Cancelar");

            if (confirmar)
                AddBoxes(cajasFinal);
        }

        // Aplica la suma o resta a los jornaleros seleccionados
        private void AddBoxes(int numberOfBoxes)
        {
            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<Jornalero>().ToList();

            if (!seleccionados.Any())
            {
                DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
                return;
            }

            foreach (var jornalero in seleccionados)
            {
                int nuevoValor = jornalero.Cajas + numberOfBoxes;
                jornalero.Cajas = nuevoValor < 0 ? 0 : nuevoValor;
                Preferences.Set($"Cajas_{jornalero.IdJornalero}", jornalero.Cajas);
            }

            DisplayAlert("Éxito",
                $"{(numberOfBoxes >= 0 ? "Se han añadido" : "Se han restado")} {Math.Abs(numberOfBoxes)} {Pluralizar("caja", Math.Abs(numberOfBoxes))} a {seleccionados.Count} {Pluralizar("jornalero", seleccionados.Count)}",
                "OK");
        }

        // Devuelve palabra en singular o plural según la cantidad
        private string Pluralizar(string palabra, int cantidad)
        {
            return cantidad == 1 ? palabra : palabra + "s";
        }

        // Construye el mensaje resumen para la confirmación

        private string CrearResumenAccion(int cajas, int cantidadJornaleros)
        {
            string verbo = cajas > 0 ? "Añadir" : "Restar";
            string palabraCaja = Pluralizar("caja", Math.Abs(cajas));
            string palabraJornalero = Pluralizar("jornalero", cantidadJornaleros);
            return $"{verbo} {Math.Abs(cajas)} {palabraCaja} a {cantidadJornaleros} {palabraJornalero}";
        }
    }

    // Modelo con soporte para notificación de cambios
    public class Jornalero : INotifyPropertyChanged
    {
        private int _cajas;

        public int IdJornalero { get; set; }
        public string Nombre { get; set; }

        public int Cajas
        {
            get => _cajas;
            set
            {
                if (_cajas != value)
                {
                    _cajas = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool TarjetaNFC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
