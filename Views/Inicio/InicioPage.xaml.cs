using Microsoft.Maui.Controls;
using AlfinfData.ViewModels;

namespace AlfinfData.Views.Inicio
{
    public partial class InicioPage : ContentPage
    {
        public InicioPage()
        {
            InitializeComponent();
            BindingContext = new InicioViewModel();
        }
    }
}
