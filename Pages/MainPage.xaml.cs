namespace CantinaOnline.Pages;

public partial class MainPage : ContentPage
{
	

    public MainPage(bool IsConnected)
	{
		InitializeComponent();
		conCheck.Text = "IsConnected: " + IsConnected;
    }



}