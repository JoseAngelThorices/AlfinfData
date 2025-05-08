using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.NFC;

namespace AlfinfData
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize |
                               ConfigChanges.Orientation |
                               ConfigChanges.UiMode)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossNFC.Init(this);

            // Inicializa Plugin.CurrentActivity
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // Cambia la barra de estado a negro
            Window.SetStatusBarColor(Android.Graphics.Color.Black);

            
            Window.SetNavigationBarColor(Android.Graphics.Color.Black);
        }

        protected override void OnResume()
        {
            base.OnResume();
            CrossNFC.OnResume();

            // Asegura que Plugin.CurrentActivity siga apuntando aquí
            CrossCurrentActivity.Current.Activity = this;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            // Reenvía el Intent NFC al plugin para que dispare OnMessageReceived
            CrossNFC.OnNewIntent(intent);
        }
    }
}
