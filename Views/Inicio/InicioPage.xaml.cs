using AlfinfData.ViewModels;

namespace AlfinfData.Views.Inicio
{
    public partial class InicioPage : ContentPage
    {
        public InicioPage(InicioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}