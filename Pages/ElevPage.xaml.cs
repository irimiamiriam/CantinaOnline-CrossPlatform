using CantinaOnline.Models;
using Plugin.Maui.Calendar.Models;
using QRCoder;

namespace CantinaOnline.Pages;

public partial class ElevPage : ContentPage
{
    public EventCollection Events;
    ElevModel _user;

    public ElevPage(ElevModel user)
    {
        InitializeComponent();

        BindingContext = new ElevPageViewModel(user);


    }


}


