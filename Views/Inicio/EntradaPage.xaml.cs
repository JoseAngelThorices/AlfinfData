using System.Diagnostics;
using System.Threading.Tasks;
using AlfinfData.ViewModels;
using Plugin.NFC;
namespace AlfinfData.Views.Inicio;

public partial class EntradaPage : ContentPage
{
    private readonly EntradaViewModel viewModel;
    public EntradaPage(EntradaViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
            BindingContext = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var resultado = await viewModel.EntradaNFCAsync();
            if( resultado == true)
            {
                await viewModel.CargarHoraAsync();
            }
            
        
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await viewModel.cancelarNFC();
        }

}

