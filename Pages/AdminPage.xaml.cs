using CantinaOnline.ViewModel;

namespace CantinaOnline.Pages;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();

            BindingContext = new AdminPageViewModel();
    }
   

}