using Camera.MAUI.ZXingHelper;
using Camera.MAUI;
using CommunityToolkit.Mvvm.Input;
using Camera.MAUI.ZXing;
using CantinaOnline.Models;


namespace CantinaOnline.Pages;

public partial class ScanPage : ContentPage
{
    private ScanPageViewModel _viewModel;
    private FirestoreService _firestoreService;
    int eatingToday;

    public ScanPage(FirestoreService firestoreService)
    {
        InitializeComponent();
        _firestoreService = firestoreService;
        //  _viewModel = new ScanPageViewModel();
        //        BindingContext = _viewModel;

        GetUserEating();

        cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
        cameraView.BarCodeOptions = new BarcodeDecodeOptions
        {
            AutoRotate = true,
            PossibleFormats = { BarcodeFormat.QR_CODE },
            ReadMultipleCodes = false,
            TryHarder = true,
            TryInverted = true
        };
        cameraView.BarCodeDetectionFrameRate = 10;
        cameraView.BarCodeDetectionMaxThreads = 5;
        cameraView.ControlBarcodeResultDuplicate = true;
        cameraView.BarCodeDetectionEnabled = true;
       
    }

    private async void GetUserEating()
    {
        eatingToday= await _firestoreService.GetUsersEatingToday();
        labelEating.Text = "Elevi care mananca azi: " + eatingToday;


    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private async void cameraView_BarcodeDetected(object sender, BarcodeEventArgs args)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            string qrId = args.Result[0].Text;
            var user = await FirestoreService.GetUserById(qrId);
            if (user != null)
            {
                DateTime today = DateTime.Now.Date;
                if (user.LastScan == today.ToString("yyyy-MM-dd"))
                {
                    barcodeResult.TextColor = Colors.Red;
                    barcodeResult.Text = "! Deja scanat azi"; 
                }
                else
                {
                    bool isPaid = user.ZilePlatite.ContainsKey(today) && user.ZilePlatite[today] == 0;
                    string message = isPaid ? "Utilizatorul a platit" : "Masa neplatita";
                    Color color = isPaid ? Colors.Green : Colors.Red;
                    barcodeResult.Text = message;
                    barcodeResult.TextColor = color;
                    if (isPaid)
                    {
                        eatingToday--;
                        labelEating.Text = "Elevi care mananca azi: " + eatingToday;


                    }
                    user.LastScan = today.ToString("yyyy-MM-dd");
                    await FirestoreService.UpdateUserLastScan(user.Id.ToString(), user.LastScan);
                }

            }
            else
            {
                barcodeResult.Text = "Nu exista elevul!";
            }

        });

    }
}