using Microsoft.Maui.Controls;

namespace AlfinfData.Views.Horas
{
    public partial class CalcularPage : ContentPage
    {
        public CalcularPage()
        {
            InitializeComponent();
        }

        // Botón de A. Selección (verde)
        private void OnSeleccionToggled(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = button.Text == "SI" ? "NO" : "SI";
            button.BackgroundColor = button.Text == "SI"
                ? Color.FromHex("#3cb043")  // Verde
                : Color.FromHex("#cdcdcd"); // Gris
        }

        // Botón de Falta (inicia vacío, alterna SI/vacío)
        private void OnFaltaToggled(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (string.IsNullOrWhiteSpace(button.Text))
            {
                // Si está vacío, poner "SI" y color verde
                button.Text = "SI";
                button.BackgroundColor = Color.FromHex("#3cb043"); // Verde
            }
            else
            {
                // Si tiene "SI", limpiar y poner gris
                button.Text = "";
                button.BackgroundColor = Color.FromHex("#cdcdcd"); // Gris
            }
        }


        private void OnNumeroClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            visorCalculadora.Text += button.Text;
        }

        private void OnCommaClicked(object sender, EventArgs e)
        {
            if (!visorCalculadora.Text.Contains(","))
                visorCalculadora.Text += ",";
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            visorCalculadora.Text = string.Empty;
        }

        private void OnGHorasClicked(object sender, EventArgs e)
        {
            // Lógica para G.HORAS
        }

        private void OnHNClicked(object sender, EventArgs e)
        {
            // Lógica para H.N.
        }

        private void OnHEClicked(object sender, EventArgs e)
        {
            // Lógica para H.E1 y H.E2
        }
    }
}
