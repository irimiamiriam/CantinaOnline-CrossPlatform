using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using CantinaOnline.Pages;

namespace CantinaOnline
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private FirestoreService firestore;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Remove title bar completely
            Window.SetFlags(
                WindowManagerFlags.LayoutNoLimits,
                WindowManagerFlags.LayoutNoLimits);

        }


    }
}
