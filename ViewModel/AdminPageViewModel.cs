using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class AdminPageViewModel : ObservableObject
{
    private int _usersEatingToday;
    public int UsersEatingToday
    {
        get => _usersEatingToday;
        set => SetProperty(ref _usersEatingToday, value);
    }

    private CameraView _cameraView;

    [ObservableProperty]
    private string barcodeResult;

    public IRelayCommand StartCameraCommand { get; }
    public IRelayCommand StopCameraCommand { get; }
    public IRelayCommand<BarcodeEventArgs> BarcodeDetectedCommand { get; }


    public AdminPageViewModel()
    {
        LoadData();
        StartCameraCommand = new AsyncRelayCommand(StartCameraAsync);
        StopCameraCommand = new AsyncRelayCommand(StopCameraAsync);
        BarcodeDetectedCommand = new RelayCommand<BarcodeEventArgs>(OnBarcodeDetected);
    }

    private async void LoadData()
    {
        UsersEatingToday = await FirestoreService.GetUsersEatingToday();
    }
    public void SetCameraView(CameraView cameraView)
    {
        _cameraView = cameraView;
    }

    private async Task StartCameraAsync()
    {
        if (_cameraView == null || _cameraView.Cameras.Count == 0)
            return;

        _cameraView.Camera = _cameraView.Cameras.First();
        await _cameraView.StopCameraAsync();
        await _cameraView.StartCameraAsync();
    }

    private async Task StopCameraAsync()
    {
        if (_cameraView != null)
        {
            await _cameraView.StopCameraAsync();
        }
    }

    private void OnBarcodeDetected(BarcodeEventArgs args)
    {
        if (args.Result?.Any() == true)
        {
            BarcodeResult = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";

            // Process scanned QR Code
            CheckUserPayment(args.Result[0].Text);
        }
    }

    private async void CheckUserPayment(string userId)
    {
        bool isPaid = await CheckIfUserPaid(userId);
        string message = isPaid ? "Utilizatorul a plătit" : "Zi neplătită";
        await Application.Current.MainPage.DisplayAlert("Rezultat Scanare", message, "OK");
    }

    private async Task<bool> CheckIfUserPaid(string userId)
    {
        var user = await FirestoreService.GetUserById(userId);
        if (user != null)
        {
            DateTime today = DateTime.Now.Date;
            return user.ZilePlatite.ContainsKey(today) && user.ZilePlatite[today] == 0;
        }
        return false;
    }

}
