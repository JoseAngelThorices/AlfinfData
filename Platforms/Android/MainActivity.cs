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

            // 2) Inicializa Plugin.CurrentActivity
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            CrossNFC.OnResume();

            // 4) Asegura que Plugin.CurrentActivity siga apuntando aquí
            CrossCurrentActivity.Current.Activity = this;
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            // 5) Reenvía el Intent NFC al plugin para que dispare OnMessageReceived
            CrossNFC.OnNewIntent(intent);
        }


    }
}
