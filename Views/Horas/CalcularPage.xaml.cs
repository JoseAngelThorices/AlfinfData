using Microsoft.Maui.Controls;

namespace AlfinfData.Views.Horas
{
    public partial class CalcularPage : ContentPage
    {
        public CalcularPage()
        {
            InitializeComponent();
        }

        // Botón de selección (SI/NO)
        private void OnSeleccionToggled(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = button.Text == "SI" ? "NO" : "SI";
            button.BackgroundColor = button.Text == "SI"
                ? Color.FromHex("#3cb043")
                : Color.FromHex("#ff4444");
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