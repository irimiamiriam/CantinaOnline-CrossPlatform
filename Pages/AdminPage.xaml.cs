namespace CantinaOnline.Pages;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
		Title = "asasdasdasdasdasdd";
	}
    private void OnElevNouClicked(object sender, EventArgs e)
    {
        // Handle "Elev nou" button click
        DisplayAlert("Elev nou", "Elev nou button clicked!", "OK");
    }

    private void OnPlataNouaClicked(object sender, EventArgs e)
    {
        // Handle "Plata noua" button click
    }

    private void OnListaEleviClicked(object sender, EventArgs e)
    {
        // Handle "Lista elevi" button click
    }

}