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

        public ICommand LoginCommand { get; }

        public MainPageViewModel(FirestoreService firestore, bool isConnected)
        {
            _firestore = firestore;
            ConnectionStatus = isConnected ? string.Empty : "Connection failed!";

            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(PasswordInput))
                return;

            ElevModel? user = await _firestore.GetElevByPassword(PasswordInput);
            AdminModel? admin = await _firestore.GetAdminByPassword(PasswordInput);

            if (user != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ElevPage(user));
            }
            else if (admin != null)
            {
                Page nextPage = admin.Rol == "Contabil" ? new ContabilPage() : new AdminPage();
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Parola greșită, încercați din nou!", "OK");
            }
        }
    }
}
