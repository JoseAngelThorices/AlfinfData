using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace AlfinfData.Views.Fin
{
    public partial class FinPage : ContentPage
    {
        private DateTime _fechaDesde = DateTime.Now;
        private DateTime _fechaHasta = DateTime.Now;

        public FinPage()
        {
            InitializeComponent();
            ActualizarFechas();
        }

        private void ActualizarFechas()
        {
            FechaDesdeLabel.Text = _fechaDesde.ToString("dd/MM/yyyy");
            FechaHastaLabel.Text = _fechaHasta.ToString("dd/MM/yyyy");
        }

        private async void OnFechaDesdeClicked(object sender, EventArgs e)
        {
            await MostrarCalendario("DESDE", _fechaDesde, (fechaSeleccionada) =>
            {
                _fechaDesde = fechaSeleccionada;
                if (_fechaDesde > _fechaHasta)
                {
                    _fechaHasta = _fechaDesde;
                }
                ActualizarFechas();
            });
        }

        private async void OnFechaHastaClicked(object sender, EventArgs e)
        {
            await MostrarCalendario("HASTA", _fechaHasta, (fechaSeleccionada) =>
            {
                _fechaHasta = fechaSeleccionada;
                if (_fechaHasta < _fechaDesde)
                {
                    _fechaDesde = _fechaHasta;
                }
                ActualizarFechas();
            });
        }

        private async Task MostrarCalendario(string titulo, DateTime fechaActual, Action<DateTime> alSeleccionar)
        {
            try
            {
                // Configurar cultura en español
                var cultura = new CultureInfo("es-ES");

                // Crear elementos del calendario
                var lblTitulo = new Label
                {
                    Text = titulo,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                };

                var lblAnio = new Label
                {
                    Text = fechaActual.Year.ToString(),
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                };

                var lblMesDia = new Label
                {
                    Text = $"{cultura.DateTimeFormat.GetDayName(fechaActual.DayOfWeek).Substring(0, 3)}, {fechaActual.Day} {cultura.DateTimeFormat.GetMonthName(fechaActual.Month).Substring(0, 3)}",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                };

                var lblMesCompleto = new Label
                {
                    Text = $"{cultura.DateTimeFormat.GetMonthName(fechaActual.Month)} de {fechaActual.Year}",
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                };

                // Crear grid para los días de la semana
                var gridDiasSemana = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition()
                    },
                    Margin = new Thickness(0, 10, 0, 5)
                };

                string[] dias = { "L", "M", "X", "J", "V", "S", "D" };
                for (int i = 0; i < 7; i++)
                {
                    gridDiasSemana.Add(new Label
                    {
                        Text = dias[i],
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    }, i, 0);
                }

                // Crear grid para los días del mes
                var gridDiasMes = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition()
                    },
                    RowDefinitions =
                    {
                        new RowDefinition(),
                        new RowDefinition(),
                        new RowDefinition(),
                        new RowDefinition(),
                        new RowDefinition(),
                        new RowDefinition()
                    }
                };

                DateTime primerDiaMes = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                int diasEnMes = DateTime.DaysInMonth(fechaActual.Year, fechaActual.Month);
                int diaSemanaInicio = (int)primerDiaMes.DayOfWeek;
                if (diaSemanaInicio == 0) diaSemanaInicio = 7; // Ajustar domingo

                int fila = 0;
                int columna = diaSemanaInicio - 1;

                for (int dia = 1; dia <= diasEnMes; dia++)
                {
                    var btnDia = new Button
                    {
                        Text = dia.ToString(),
                        BackgroundColor = Colors.Transparent,
                        TextColor = dia == fechaActual.Day ? Colors.White : Colors.Black,
                        CornerRadius = 20,
                        WidthRequest = 40,
                        HeightRequest = 40
                    };

                    if (dia == fechaActual.Day)
                    {
                        btnDia.BackgroundColor = Color.FromArgb("#71639e");
                    }

                    int diaActual = dia; // Capturar valor para el closure
                    btnDia.Clicked += (s, e) =>
                    {
                        var nuevaFecha = new DateTime(fechaActual.Year, fechaActual.Month, diaActual);
                        alSeleccionar?.Invoke(nuevaFecha);
                        Navigation.PopModalAsync();
                    };

                    gridDiasMes.Add(btnDia, columna, fila);

                    columna++;
                    if (columna > 6)
                    {
                        columna = 0;
                        fila++;
                    }
                }

                // Botones de acción
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

                btnCancelar.Clicked += (s, e) => Navigation.PopModalAsync();
                btnAceptar.Clicked += (s, e) =>
                {
                    alSeleccionar?.Invoke(fechaActual);
                    Navigation.PopModalAsync();
                };

                // Diseño completo
                var stackLayout = new VerticalStackLayout
                {
                    BackgroundColor = Colors.White,
                    Padding = 20,
                    Spacing = 10,
                    Children =
                    {
                        lblTitulo,
                        lblAnio,
                        lblMesDia,
                        new BoxView { HeightRequest = 1, Color = Colors.LightGray },
                        lblMesCompleto,
                        gridDiasSemana,
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
                    Content = stackLayout,
                    BackgroundColor = Colors.Transparent
                };

                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al mostrar calendario: {ex.Message}", "OK");
            }
        }
    }
}