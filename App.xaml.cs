using AlfinfData.Views;

namespace AlfinfData
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light; //Tema Claro para todo el sistema.


        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}