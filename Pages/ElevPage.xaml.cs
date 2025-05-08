using CantinaOnline.Models;
using CantinaOnline.ViewModel;
using Plugin.Maui.Calendar.Models;
using QRCoder;

namespace CantinaOnline.Pages;

public partial class ElevPage : ContentPage
{
    public EventCollection Events;

    public ElevPage(ElevModel user)
    {
        InitializeComponent();

        var vm = new NewElevPageViewModel(user);
        BindingContext = vm;
        


    }
    private bool _isQrExpanded = false;

    private async void OnQrCodeTapped(object sender, EventArgs e)
    {
        if (_isQrExpanded)
        {
            // Hide overlay
            await QrOverlay.FadeTo(0, 200);
            QrOverlay.IsVisible = false;
            MainContent.IsVisible = true;
        }
        else
        {
            // Show overlay
            MainContent.IsVisible = false;
            QrOverlay.Opacity = 0;
            QrOverlay.IsVisible = true;
            await QrOverlay.FadeTo(1, 200);
        }

        _isQrExpanded = !_isQrExpanded;

    }


}


