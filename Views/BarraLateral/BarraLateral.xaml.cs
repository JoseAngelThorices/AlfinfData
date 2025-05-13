using Microsoft.Maui.Controls;
using System;

namespace AlfinfData.Views.BarraLateral
{
    public partial class BarraLateral : VerticalStackLayout
    {
        public BarraLateral()
        {
            InitializeComponent();
            BindingContext = Shell.Current;
        }

        private async void OnAccesoDirectoAltaNFC(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("DescargasPage?accion=alta");
        }
    }
}
