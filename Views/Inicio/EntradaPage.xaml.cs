namespace AlfinfData.Views.Inicio;

public partial class EntradaPage : ContentPage
{
    const string HoraGuardadaKey = "HoraSeleccionada";

        public EntradaPage()
        {
            InitializeComponent();
            RecuperarHoraGuardada();
        }

        private void RecuperarHoraGuardada()
        {
            var hora = Preferences.Get(HoraGuardadaKey, string.Empty);
            if (!string.IsNullOrEmpty(hora))
            {
                HoraButton.Text = hora;
            }
        }

        private async void OnHoraButtonClicked(object sender, EventArgs e)
        {
            string[] horas = new string[48];
            for (int i = 0; i < 24; i++)
            {
                horas[i * 2] = $"{i:D2}:00";
                horas[i * 2 + 1] = $"{i:D2}:30";
            }

            string seleccion = await DisplayActionSheet("Selecciona hora", "Cancelar", null, horas);

            if (!string.IsNullOrEmpty(seleccion) && seleccion != "Cancelar")
            {
                HoraButton.Text = seleccion;
                Preferences.Set(HoraGuardadaKey, seleccion);
            }
        }
    }

