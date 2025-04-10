using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using CantinaOnline.Models;
using CantinaOnline.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace CantinaOnline.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly FirestoreService _firestore;

        [ObservableProperty]
        private string passwordInput;

        [ObservableProperty]
        private string connectionStatus;

        [ObservableProperty]
        private bool rememberMeChecked;

        public ICommand LoginCommand { get; }

        public MainPageViewModel(FirestoreService firestore, bool isConnected)
        {
            _firestore = firestore;
            ConnectionStatus = isConnected ? string.Empty : "Connection failed!";
            var passSaved = SecureStorage.GetAsync("userpass");
            if (passSaved != null)
            {
                PasswordInput = passSaved.Result;
            }
            LoginCommand = new AsyncRelayCommand(LoginAsync);
           
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(PasswordInput))
                return;

            ElevModel? user = await _firestore.GetElevByPassword(PasswordInput);
            ScanModel? scan = await _firestore.GetScanByPassword(PasswordInput);
            AdminModel? admin = await _firestore.GetAdminByPassword(PasswordInput);

            if (user != null)
            {
              if (RememberMeChecked)
                {
                    await SecureStorage.SetAsync("userpass", PasswordInput);
                }

                await Application.Current.MainPage.Navigation.PushAsync(new ElevPage(user));
            }
            else if (scan != null)
            {
                if (RememberMeChecked)
                {
                    await SecureStorage.SetAsync("userpass", PasswordInput);
                }

                Page nextPage = scan.Rol == "Contabil" ? new ContabilPage() : new ScanPage(_firestore);
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
            else
            {
                if (RememberMeChecked)
                {
                    await SecureStorage.SetAsync("userpass", PasswordInput);
                }

                await Application.Current.MainPage.DisplayAlert("Eroare", "Parola greșită, încercați din nou!", "OK");
            }
        }
    }
}
