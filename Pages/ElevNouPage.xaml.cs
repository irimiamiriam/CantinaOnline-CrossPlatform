namespace CantinaOnline.Pages;

public partial class ElevNouPage : ContentPage
{
	public ElevNouPage()
	{
		InitializeComponent();
		BindingContext = new ElevNouViewModel();
        NavigationPage.SetHasNavigationBar(this, false);

    }
}