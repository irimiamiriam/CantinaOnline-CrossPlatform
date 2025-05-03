using Camera.MAUI.ZXingHelper;
using Camera.MAUI;
using CommunityToolkit.Mvvm.Input;
using Camera.MAUI.ZXing;
using CantinaOnline.Models;

using Application = Microsoft.Maui.Controls.Application;

namespace CantinaOnline.Pages;

public partial class AdminPage : ContentPage
{
    private ScanPageViewModel _viewModel;
    private FirestoreService _firestoreService;

    public AdminPage()
    {
		InitializeComponent();

    }
    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new ElevNouPage());
    
    }
}