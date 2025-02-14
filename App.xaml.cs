using CantinaOnline.Pages;

namespace CantinaOnline
{
    public partial class App : Application
    {
        public App(FirestoreService firestoreService)
        {
            InitializeComponent();

            MainPage = new SplashScreen(firestoreService);
        }
    }
}
