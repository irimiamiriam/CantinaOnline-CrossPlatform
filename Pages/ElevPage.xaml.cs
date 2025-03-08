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
        user.ZilePlatite= user.ZilePlatite.OrderBy(r => r).ToList();
        calendarView.SelectedDates = user.ZilePlatite;
        _user = user;
        monthPicker.ItemsSource = Enumerable.Range(1, 12).Select(m => m.ToString()).ToList();

        monthPicker.SelectedItem = DateTime.Now.Month;
        UpdateDayPicker();
        numeL.Text = user.Nume;
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(user.Id.ToString(), QRCodeGenerator.ECCLevel.L);
        PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));


        // Get the current year and month
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;

        // Get the total number of days in the current month
        int daysInMonth = DateTime.DaysInMonth(year, month);

        // Initialize Events collection
        Events = new EventCollection();

        // Loop through all days of the month
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(year, month, day);

            // Check if the day is Saturday (6) or Sunday (0)
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                Events[date] = new List<EventModel>
        {
            new EventModel { Name = "", Description = "" } // No text
        };
            }
        }


        calendarView.Events = Events;

    }
    private void UpdateDayPicker()
    {
        if (monthPicker.SelectedItem != null)
        {
            int selectedMonth = int.Parse(monthPicker.SelectedItem.ToString());
            var daysInSelectedMonth = _user.ZilePlatite
                .Where(date => date.Month == selectedMonth)
                .Select(date => date.Day.ToString())
                .ToList();

            dayPicker.ItemsSource = daysInSelectedMonth;
        }
    }
    private void OnAdaugaRestantaClicked(object sender, EventArgs e)
    {
        if (dayPicker.SelectedItem == null || monthPicker.SelectedItem == null)
        {
            DisplayAlert("Eroare", "Te rog s? selectezi o zi ?i o lun?.", "OK");
            return;
        }

        int selectedDay = int.Parse(dayPicker.SelectedItem.ToString());
        int selectedMonth = int.Parse(monthPicker.SelectedItem.ToString());
        int currentYear = DateTime.Now.Year;

        DateTime selectedDate = new DateTime(currentYear, selectedMonth, selectedDay);

        // Ensure the date is part of ZilePlatite
        //if (!_user.ZilePlatite.Contains(selectedDate))
        //{
        //    DisplayAlert("Eroare", "Data selectat? nu este valid?.", "OK");
        //    return;
        //}

        // Add event on selected date
        if (!Events.ContainsKey(selectedDate))
        {
            Events[selectedDate] = new List<EventModel>();
        }
        Events[selectedDate] = new List<EventModel> { new EventModel() { Name = "Restan??", Description = "Restan?? ad?ugat?" } };

        // Refresh Calendar
        calendarView.Events = Events;
    }
}


