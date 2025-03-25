using CantinaOnline.Pages;
using Microsoft.Maui.Controls;

namespace CantinaOnline
{
    public partial class App : Application
    {
        private readonly FirestoreService firestore;
        private Window _mainWindow;

        public App(FirestoreService firestore)
        {
            InitializeComponent();
            this.firestore = firestore;

            // Initialize with a temporary page while checking connection
            _mainWindow = new Window(new ContentPage
            {
                Content = new ActivityIndicator { IsRunning = true }
            });

            // Start connection check
            CheckConnection();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return _mainWindow;
        }

        private async void CheckConnection()
        {
            await Task.Delay(2000);

            bool isConnected = IsConnectedToInternet() && firestore.CheckConnection();

            // Update the window's page on the main thread
            Dispatcher.Dispatch(() =>
            {
                _mainWindow.Page = new NavigationPage(new MainPage(isConnected, firestore));
            });
        }

        private bool IsConnectedToInternet()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}