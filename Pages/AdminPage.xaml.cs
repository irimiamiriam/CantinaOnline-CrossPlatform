using CantinaOnline.ViewModel;

namespace CantinaOnline.Pages;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();

            BindingContext = new AdminPageViewModel();
        DisplayAlert("Plata noua", "Plata noua button clicked!", "OK");
   
        DisplayAlert("Lista elevi", "Lista elevi button clicked!", "OK");
    }

}