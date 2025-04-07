using AlfinfData.ViewModels;

namespace AlfinfData.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(); 
        }
    }
}
