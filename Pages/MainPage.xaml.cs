using CantinaOnline.Models;
using Google.Cloud.Firestore.V1;

namespace CantinaOnline.Pages;

public partial class MainPage : ContentPage
{
	FirestoreService firestore;

    public MainPage(bool IsConnected, FirestoreService firestore)
	{

		InitializeComponent();
		 this.firestore=firestore;
		if (!IsConnected)
		{
			conCheck.Text = "Connection failed!";
		}else
		{
			conCheck.IsVisible = false;
		}
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        ElevModel user = await firestore.GetElevByPassword(passwordInput.Text);
		if (user != null)
		{
			ElevPage ep = new ElevPage();
			Application.Current.MainPage = ep;
		}
    }
}