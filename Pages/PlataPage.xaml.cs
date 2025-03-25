namespace CantinaOnline.Pages;

public partial class PlataPage : ContentPage
{
	public PlataPage()
	{
		InitializeComponent();
		BindingContext = new PlataPageViewModel();
	}
}