using CantinaOnline.ViewModels;

namespace CantinaOnline.Pages;

public partial class CalendarCantinaPage : ContentPage
{
	public CalendarCantinaPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        BindingContext = new CalendarCantinaModelView();
	}
} 