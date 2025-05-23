using System.Globalization;
using AlfinfData.ViewModels;

namespace AlfinfData.Views.Fin
{
    public partial class FinPage : ContentPage
    {
        private readonly FinViewModel _viewModel;

        public FinPage(FinViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;

            _viewModel.FechaDesde = DateTime.Now;
            _viewModel.FechaHasta = DateTime.Now;

            ActualizarFechas();
        }

        private void ActualizarFechas()
        {
            FechaDesdeLabel.Text = _viewModel.FechaDesde.ToString("dd/MM/yyyy");
            FechaHastaLabel.Text = _viewModel.FechaHasta.ToString("dd/MM/yyyy");
        }



        private async void OnFechaDesdeClicked(object sender, EventArgs e)
        {
            await MostrarCalendario("DESDE", _viewModel.FechaDesde, (fechaSeleccionada) =>
            {
                _viewModel.FechaDesde = fechaSeleccionada;

                if (_viewModel.FechaDesde > _viewModel.FechaHasta)
                    _viewModel.FechaHasta = _viewModel.FechaDesde;

                ActualizarFechas();
            });
        }


        private async void OnFechaHastaClicked(object sender, EventArgs e)
        {
            await MostrarCalendario("HASTA", _viewModel.FechaHasta, (fechaSeleccionada) =>
            {
                _viewModel.FechaHasta = fechaSeleccionada;

                if (_viewModel.FechaHasta < _viewModel.FechaDesde)
                    _viewModel.FechaDesde = _viewModel.FechaHasta;

                ActualizarFechas();
            });
        }
        private async Task MostrarCalendario(string titulo, DateTime fechaInicial, Action<DateTime> alSeleccionar)
        {
            try
            {
                var cultura = new CultureInfo("es-ES");
                DateTime hoy = DateTime.Today;
                DateTime fechaActual = fechaInicial;
                DateTime fechaSeleccionada = hoy;

                int añoInstalacion = Preferences.Get("AñoInicio", hoy.Year);
                int mesInstalacion = Preferences.Get("MesInicio", hoy.Month);

                var lblTitulo = new Label
                {
                    Text = titulo,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                };

                // Pickers
                var pickerMes = new Picker { Title = "Mes", WidthRequest = 120 };
                var pickerAnio = new Picker { Title = "Año", WidthRequest = 100 };

                // Años desde instalación hasta hoy
                var anios = Enumerable.Range(añoInstalacion, hoy.Year - añoInstalacion + 1).ToList();
                pickerAnio.ItemsSource = anios;
                pickerAnio.SelectedItem = fechaActual.Year;

                void ActualizarMeses()
                {
                    int anioSeleccionado = (int)(pickerAnio.SelectedItem ?? hoy.Year);

                    var mesesDisponibles = Enumerable.Range(1, 12)
                        .Where(m => !(anioSeleccionado == añoInstalacion && m < mesInstalacion))
                        .Select(m => new DateTime(1, m, 1).ToString("MMMM", cultura))
                        .ToList();

                    pickerMes.ItemsSource = mesesDisponibles;
                    if (anioSeleccionado == fechaActual.Year)
                        pickerMes.SelectedIndex = fechaActual.Month - mesInstalacion;
                    else
                        pickerMes.SelectedIndex = 0;
                }

                var gridDiasMes = new Grid();
                for (int i = 0; i < 7; i++)
                    gridDiasMes.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < 6; i++)
                    gridDiasMes.RowDefinitions.Add(new RowDefinition());

                async void ActualizarDias()
                {
                    gridDiasMes.Children.Clear();

                    int anio = (int)(pickerAnio.SelectedItem ?? hoy.Year);
                    int mes = pickerMes.SelectedIndex + 1;
                    if (anio == añoInstalacion)
                        mes += mesInstalacion - 1;

                    var primerDia = new DateTime(anio, mes, 1);
                    int diasEnMes = DateTime.DaysInMonth(anio, mes);
                    int diaSemanaInicio = (int)primerDia.DayOfWeek;
                    if (diaSemanaInicio == 0) diaSemanaInicio = 7;

                    int fila = 0, columna = diaSemanaInicio - 1;

                    for (int dia = 1; dia <= diasEnMes; dia++)
                    {
                        var fechaBtn = new DateTime(anio, mes, dia);
                        var btn = new Button
                        {
                            Text = dia.ToString(),
                            BackgroundColor = Colors.Transparent,
                            TextColor = Colors.Black,
                            CornerRadius = 20,
                            WidthRequest = 40,
                            HeightRequest = 40,
                            IsEnabled = fechaBtn <= hoy
                        };

                        if (fechaBtn == hoy)
                        {
                            btn.BackgroundColor = Color.FromArgb("#71639e");
                            btn.TextColor = Colors.White;
                            fechaSeleccionada = fechaBtn;
                        }

                        btn.Clicked += (s, e) =>
                        {
                            fechaSeleccionada = fechaBtn;

                            foreach (var b in gridDiasMes.Children.OfType<Button>())
                            {
                                b.BackgroundColor = Colors.Transparent;
                                b.TextColor = Colors.Black;
                            }

                            btn.BackgroundColor = Color.FromArgb("#71639e");
                            btn.TextColor = Colors.White;
                        };

                        gridDiasMes.Add(btn, columna, fila);
                        columna++;
                        if (columna > 6)
                        {
                            columna = 0;
                            fila++;
                        }
                    }
                }

                pickerAnio.SelectedIndexChanged += (s, e) =>
                {
                    ActualizarMeses();
                    ActualizarDias();
                };
                pickerMes.SelectedIndexChanged += (s, e) => ActualizarDias();

                var btnCancelar = new Button
                {
                    Text = "CANCELAR",
                    BackgroundColor = Colors.LightGray,
                    TextColor = Colors.Black,
                    CornerRadius = 5,
                    Margin = new Thickness(5)
                };

                var btnAceptar = new Button
                {
                    Text = "ACEPTAR",
                    BackgroundColor = Color.FromArgb("#71639e"),
                    TextColor = Colors.White,
                    CornerRadius = 5,
                    Margin = new Thickness(5)
                };

                btnCancelar.Clicked += async (s, e) => await Navigation.PopModalAsync();
                btnAceptar.Clicked += async (s, e) =>
                {
                    alSeleccionar?.Invoke(fechaSeleccionada);
                    await Navigation.PopModalAsync();
                };

                var layout = new VerticalStackLayout
                {
                    BackgroundColor = Colors.White,
                    Padding = 20,
                    Spacing = 10,
                    Children =
            {
                lblTitulo,
                new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { pickerMes, pickerAnio }
                },
                gridDiasMes,
                new BoxView { HeightRequest = 1, Color = Colors.LightGray },
                new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Children = { btnCancelar, btnAceptar }
                }
            }
                };

                var page = new ContentPage
                {
                    Content = layout,
                    BackgroundColor = Colors.Transparent
                };

                await Navigation.PushModalAsync(page);

                // Inicializar meses y días
                ActualizarMeses();
                ActualizarDias();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al mostrar calendario: {ex.Message}", "OK");
            }
        }

    }
}