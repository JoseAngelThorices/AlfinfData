using AlfinfData.ViewModels;
using AlfinfData.Models.SQLITE;

namespace AlfinfData.Views.Produccion
{
    public partial class ProduccionPage : ContentPage
    {
        private readonly ProduccionViewModel _viewModel;

        public ProduccionPage(ProduccionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Asociar botones de cantidades fijas
            Btn1.Clicked += (s, e) => OnCantidadFijaClicked(1);
            Btn2.Clicked += (s, e) => OnCantidadFijaClicked(2);
            Btn3.Clicked += (s, e) => OnCantidadFijaClicked(3);
            Btn4.Clicked += (s, e) => OnCantidadFijaClicked(4);
            Btn5.Clicked += (s, e) => OnCantidadFijaClicked(5);

            // Botón de cantidad personalizada
            BtnN.Clicked += OnBtnNClicked;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCuadrillaAsync();
            await _viewModel.CargarJornalerosConCajasAsync();
        }

        private async void OnCantidadFijaClicked(int cantidad)
        {
            string accion = await DisplayActionSheet(
                $"¿Qué deseas hacer con {cantidad} {Pluralizar("caja", cantidad)}?",
                "Cancelar", null, "Añadir cajas", "Restar cajas");

            if (accion == "Cancelar" || string.IsNullOrWhiteSpace(accion))
                return;

            bool esSuma = accion == "Añadir cajas";
            int cajasFinal = esSuma ? cantidad : -cantidad;

            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<JornaleroConCajas>().ToList();
            if (!seleccionados.Any())
            {
                await DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
                return;
            }

            string resumen = CrearResumenAccion(cajasFinal, seleccionados.Count);
            bool confirmar = await DisplayAlert("Confirmar acción", resumen, "OK", "Cancelar");

            if (confirmar)
                await AplicarCambioDeCajasAsync(cajasFinal, seleccionados);
        }

        private async void OnBtnNClicked(object? sender, EventArgs e)
        {
            string accion = await DisplayActionSheet("¿Qué deseas hacer?", "Cancelar", null, "Añadir cajas", "Restar cajas");

            if (accion == "Cancelar" || string.IsNullOrWhiteSpace(accion))
                return;

            bool esSuma = accion == "Añadir cajas";
            string promptTitle = esSuma ? "Número de cajas a añadir" : "Número de cajas a restar";

            string result = await DisplayPromptAsync(promptTitle, "Introduce el número de cajas:", "Aceptar", "Cancelar", "0", maxLength: 3, keyboard: Keyboard.Numeric);

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
                await AplicarCambioDeCajasAsync(cajasFinal, seleccionados);
        }

        private async Task AplicarCambioDeCajasAsync(int cantidad, List<JornaleroConCajas> seleccionados)
        {
            foreach (var j in seleccionados)
            {
                if (j.TotalCajas + cantidad < 0)
                {
                    cantidad = -j.TotalCajas;
                }
            }

            _viewModel.SetSeleccionados(seleccionados);

            await _viewModel.ProcesarCajasAsync(cantidad);

            await DisplayAlert("Éxito",
                $"{(cantidad >= 0 ? "Se han añadido" : "Se han restado")} " +
                $"{Math.Abs(cantidad)} {Pluralizar("caja", Math.Abs(cantidad))} " +
                $"a {seleccionados.Count} {Pluralizar("jornalero", seleccionados.Count)}",
                "OK");

            ListaDeJornaleros.SelectedItems.Clear();
        }

        private string Pluralizar(string palabra, int cantidad)
        {
            return cantidad == 1 ? palabra : palabra + "s";
        }

        private string CrearResumenAccion(int cajas, int cantidadJornaleros)
        {
            string verbo = cajas > 0 ? "añadir" : "restar";
            return $"{verbo} {Math.Abs(cajas)} {Pluralizar("caja", Math.Abs(cajas))} a {cantidadJornaleros} {Pluralizar("jornalero", cantidadJornaleros)}";
        }
    }
}
