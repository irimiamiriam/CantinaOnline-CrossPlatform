using CantinaOnline.Models;
using CantinaOnline.ViewModel;
using Plugin.Maui.Calendar.Models;
using QRCoder;

namespace CantinaOnline.Pages;

public partial class ElevPage : ContentPage
{
    public EventCollection Events;

    public ElevPage(ElevModel user)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);


        // var vm = new ElevPageViewModel(user);
        //vm.ForceCalendarRefresh = () =>
        //{
        //    int current = calendarView.Month;
        //    calendarView.Month = current + 1;
        //    calendarView.Month = current;
        //};

        var vm = new NewElevPageViewModel(user);
        BindingContext = vm;
        


    }


}


