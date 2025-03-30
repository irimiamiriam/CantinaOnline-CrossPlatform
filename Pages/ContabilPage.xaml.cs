namespace CantinaOnline.Pages;

public partial class ContabilPage : ContentPage
{
	public ContabilPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        VerifyAndUpdateZileCantina();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new PlataPage());
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new CalendarCantinaPage());

    }
    private async void VerifyAndUpdateZileCantina()
    {
        try
        {
            Dictionary<string, int> zileCantina = await FirestoreService.GetAdminZileCantina();

            if (zileCantina == null || zileCantina.Count == 0)
            {
                await GenerateNewZileCantina(DateTime.Now);
                return;
            }

            // Get the last date in ZileCantina
            List<DateTime> existingDates = zileCantina.Keys.Select(date => DateTime.Parse(date)).ToList();
            DateTime lastDate = existingDates.Max();

            if (lastDate <= DateTime.Now.Date)
            {
                await GenerateNewZileCantina(lastDate.AddDays(1));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying ZileCantina: {ex.Message}");
        }
    }

    private async Task GenerateNewZileCantina(DateTime startDate)
    {
        Dictionary<string, int> newZileCantina = new();
        int daysAdded = 0;

        while (daysAdded < 30)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                newZileCantina[startDate.ToString("yyyy-MM-dd")] = 0;
                daysAdded++;
            }
            startDate = startDate.AddDays(1);
        }

        await FirestoreService.UpdateAdminZileCantina(newZileCantina);
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new ElevNouPage());

    }
}