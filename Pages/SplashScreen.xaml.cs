namespace CantinaOnline.Pages;

public partial class SplashScreen : ContentPage
{

    private readonly FirestoreService firestore;

    public SplashScreen(FirestoreService firestore)
    {
        InitializeComponent();
        this.firestore = firestore;
        CheckConnection();
    }

    private async void CheckConnection()
    {
      
        bool IsConnected = IsConnectedToInternet() && firestore.CheckConnection();
        MainPage main = new MainPage(IsConnected, firestore);
        await Task.Delay(2000);
        
        Application.Current.MainPage = main;
    }


    private bool IsConnectedToInternet()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }


}