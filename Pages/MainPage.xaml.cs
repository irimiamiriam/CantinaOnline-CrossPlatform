using CantinaOnline.Models;
using Google.Cloud.Firestore.V1;

namespace CantinaOnline.Pages;

public partial class MainPage : ContentPage
{
	private FirestoreService _firestore;

    public MainPage(bool IsConnected, FirestoreService firestore)
	{

		InitializeComponent();

        _firestore = firestore;
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

			ElevModel? user = await GetElev();
			AdminModel? admin = await GetAdmin();
			if (user != null)
			{
				ElevPage ep = new ElevPage(user);

                await Navigation.PushAsync(ep);
            }
            else
			{
				if (admin == null)
				{
					await Application.Current.MainPage.DisplayAlert("Error", "Parola gresita , incercati din nou!", "OK");
				}
				else
				{
					if (admin.Rol == "Contabil")
					{
						ContabilPage cp = new ContabilPage();
                        await Navigation.PushAsync(cp);

                    }
                    else
					{
						AdminPage ap = new AdminPage();
						await Navigation.PushAsync(ap);

                    }
                }
            }
		}
	}

    private async Task<AdminModel?> GetAdmin()
    {
		
		return await _firestore.GetAdminByPassword(passwordInput.Text);
    }

    private async Task<ElevModel?> GetElev()
    {
      
		return await _firestore.GetElevByPassword(passwordInput.Text);
    }
}