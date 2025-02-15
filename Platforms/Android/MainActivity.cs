using Android.App;
using Android.Content.PM;
using Android.OS;
using CantinaOnline.Pages;

namespace CantinaOnline
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //private FirestoreService firestore;

       

        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //    firestore = new FirestoreService();
        //    CheckConnection();
        //}

        //private async void CheckConnection()
        //{

        //    bool IsConnected = IsConnectedToInternet() && firestore.CheckConnection();
        //    MainPage main = new MainPage(IsConnected);
        //    await Task.Delay(2000);


        //    Microsoft.Maui.Controls.Application.Current.MainPage = main;
        
        //}


        //private bool IsConnectedToInternet()
        //{
        //    return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        //}

    }
}
