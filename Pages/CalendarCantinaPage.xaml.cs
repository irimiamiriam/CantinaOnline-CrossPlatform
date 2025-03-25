using CantinaOnline.ViewModels;

namespace CantinaOnline.Pages;

public partial class CalendarCantinaPage : ContentPage
{
	public CalendarCantinaPage()
	{
		InitializeComponent();
		
		BindingContext = new CalendarCantinaModelView();
	}
} 