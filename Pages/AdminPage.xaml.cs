using Camera.MAUI.ZXingHelper;
using Camera.MAUI;
using CommunityToolkit.Mvvm.Input;


namespace CantinaOnline.Pages;

public partial class AdminPage : ContentPage
{
    private AdminPageViewModel _viewModel;

    public AdminPage()
    {
        InitializeComponent();
        _viewModel = new AdminPageViewModel();
        BindingContext = _viewModel;
        _viewModel.SetCameraView(qrCamera);
    }

    private async void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        await ((AsyncRelayCommand)_viewModel.StartCameraCommand).ExecuteAsync(null);
    }

    private void cameraView_BarcodeDetected(object sender, BarcodeEventArgs args)
    {
        _viewModel.BarcodeDetectedCommand.Execute(args);
    }
}
