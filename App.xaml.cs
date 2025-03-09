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

            // Start the connection check process
            CheckConnection();
        }

        private async void CheckConnection()
        {
            await Task.Delay(2000);

            // Check network and Firestore connectivity
            bool isConnected = IsConnectedToInternet() && firestore.CheckConnection();

            // Navigate to MainPage
            MainPage = new MainPage(isConnected, firestore);
        }

        private bool IsConnectedToInternet()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
