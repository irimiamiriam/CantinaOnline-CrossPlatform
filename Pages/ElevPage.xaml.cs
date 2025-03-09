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

        BindingContext = user;
        _user = user;
        numeL.Text = user.Nume;

        calendarView.SelectedDates = user.ZilePlatite.Keys.ToList();
        monthPicker.ItemsSource = user.ZilePlatite.Keys.ToList();

        GenerateQR();
        RemoveDays();


       
    }


    private void RemoveDays()
    {
        DateTime dataInceput = _user.ZilePlatite.Keys.Min(); 
        DateTime dataFinal = _user.ZilePlatite.Keys.Max();
        Events = new EventCollection();

         while (dataInceput.Date != dataFinal.Date)
        {
            dataInceput = dataInceput.AddDays(1);
            if (!_user.ZilePlatite.Any(d => d.Key.Date == dataInceput.Date))
            {
                Events[dataInceput] = new List<EventModel>
        {
            new EventModel { Name = "", Description = "" }
        };
            }
            

        }
        calendarView.Events = Events;

    }


    private void GenerateQR()
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(_user.Id.ToString(), QRCodeGenerator.ECCLevel.L);
        PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }

    
    private void OnAdaugaRestantaClicked(object sender, EventArgs e)
    {
        if ( monthPicker.SelectedItem == null)
        {
            DisplayAlert("Eroare", "Te rog sa selectezi o  data.", "OK");
            return;
        }



        DateTime selectedDate = Convert.ToDateTime(monthPicker.SelectedItem);

        FirestoreService.UpdateZilePlatite(selectedDate, _user);
      

        // Add event on selected date
        if (!Events.ContainsKey(selectedDate))
        {
            Events[selectedDate] = new List<EventModel>();
        }
        Events[selectedDate] = new List<EventModel> { new EventModel() { Name = "Restanta", Description = "" } };

        // Refresh Calendar
        calendarView.Events = Events;
    }
}


