using CantinaOnline.Pages;

namespace CantinaOnline
{
    public partial class App : Application
    {
        private readonly FirestoreService firestore;

        public App(FirestoreService firestore)
        {
            InitializeComponent();
            this.firestore = firestore;
            MainPage = new ContentPage();

            //FirestoreService.UpdateUserZilePlatite();
            CheckConnection();
        }

        private async void CheckConnection()
        {
            await Task.Delay(2000);

            bool isConnected = IsConnectedToInternet() && firestore.CheckConnection();

            MainPage = new NavigationPage(new MainPage(isConnected, firestore));
            
        }

        private bool IsConnectedToInternet()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
