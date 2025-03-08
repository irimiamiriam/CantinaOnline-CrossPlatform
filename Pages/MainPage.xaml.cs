using CantinaOnline.Models;
using Google.Cloud.Firestore.V1;

namespace CantinaOnline.Pages;

public partial class MainPage : ContentPage
{
	private FirestoreService _firestore;

    public MainPage(bool IsConnected, FirestoreService firestore)
	{

		InitializeComponent();
		 _firestore=firestore;
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
		if (passwordInput.Text is not null)
		{


			ElevModel user = await _firestore.GetElevByPassword(passwordInput.Text);
			if (user != null)
			{
				ElevPage ep = new ElevPage(user);
				
				Application.Current.MainPage = ep;
			}
			else
			{
                await Application.Current.MainPage.DisplayAlert("Error", "Wrong password. Please try again.", "OK");
            }
		}
	}
}