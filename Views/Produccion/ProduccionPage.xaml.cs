using System.Collections.ObjectModel;
using AlfinfData.ViewModels;
using AlfinfData.Models.SQLITE;
using System;


namespace AlfinfData.Views.Produccion
{
    public partial class ProduccionPage : ContentPage
    {
        private readonly ProduccionViewModel _viewModel;

        public ProduccionPage(
            ProduccionViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
            BindingContext = _viewModel = viewModel;

            Btn1.Clicked += (s, e) => OnCantidadFijaClicked(1);
            Btn2.Clicked += (s, e) => OnCantidadFijaClicked(2);
            Btn3.Clicked += (s, e) => OnCantidadFijaClicked(3);
            Btn4.Clicked += (s, e) => OnCantidadFijaClicked(4);
            Btn5.Clicked += (s, e) => OnCantidadFijaClicked(5);
            BtnN.Clicked += OnBtnNClicked;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.CargarJornalerosConCajasAsync();
            await _viewModel.CargarCuadrillaAsync();
        }



        private async void OnCantidadFijaClicked(int cantidad)
        {
            string accion = await DisplayActionSheet(
                $"¿Qué deseas hacer con {cantidad} {pluralizar("caja", cantidad)}?",
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

            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<JornaleroConCajas>().ToList();
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

            private async void AddBoxes(int numberOfBoxes)
        {
            var seleccionados = ListaDeJornaleros
                .SelectedItems
                .Cast<JornaleroConCajas>()
                .ToList();

            if (!seleccionados.Any())
            {
                await DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
                return;
            }

            await DisplayAlert("Éxito",
                $"{(numberOfBoxes >= 0 ? "Se han añadido" : "Se han restado")} " +
                $"{Math.Abs(numberOfBoxes)} {pluralizar("caja", Math.Abs(numberOfBoxes))} " +
                $"a {seleccionados.Count} {pluralizar("jornalero", seleccionados.Count)}",
                "OK");
        }

        private string pluralizar(string palabra, int cantidad)

        {
            return cantidad == 1 ? palabra : palabra + "s";
        }

        private string CrearResumenAccion(int cajas, int cantidadjornaleros)
        {
            string verbo = cajas > 0 ? "añadir" : "restar";
            string palabracaja = pluralizar("caja", Math.Abs(cajas));
            string palabrajornalero = pluralizar("jornalero", cantidadjornaleros);
            return $"{verbo} {Math.Abs(cajas)} {palabracaja} a {cantidadjornaleros} {palabrajornalero}";
        }
    }
}
