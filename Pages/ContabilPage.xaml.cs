namespace CantinaOnline.Pages;

public partial class ContabilPage : ContentPage
{
	public ContabilPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new PlataPage());
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new CalendarCantinaPage());

    }
}