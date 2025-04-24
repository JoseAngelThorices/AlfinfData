using AlfinfData.ViewModels;
namespace AlfinfData.Views.Inicio;

public partial class DescargasPage : ContentPage
{
	public DescargasPage(DescargasViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}