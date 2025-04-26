using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CantinaOnline.Models;
using Plugin.Maui.Calendar.Models;
using QRCoder;
using System.Globalization;
using CantinaOnline;


public  class ElevPageViewModel : INotifyPropertyChanged
{
    private ElevModel _user;
    private EventCollection _events;
    public CultureInfo Culture => new CultureInfo("ro-RO");
    public Action ForceCalendarRefresh { get; set; }

    private ImageSource _qrCodeImage;
    public ImageSource QrCodeImage
    {
        get => _qrCodeImage;
        set
        {
            _qrCodeImage = value;
            OnPropertyChanged();
        }
    }

    private List<DateTime> _selectedDates;
  
    public List<DateTime> SelectedDates
    {
        get => _selectedDates;
        set
        {
            _selectedDates = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Nume => _user.Nume;

    public EventCollection Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }

    public List<DateTime> ZilePlatiteKeys => _user.ZilePlatite.Keys.ToList();

    public ICommand DayTappedCommand { get; }


    public ElevPageViewModel(ElevModel user)
    {
        _user = user;
        _events = new EventCollection();


        DayTappedCommand = new RelayCommand<DateTime>(OnDayTapped);
        GenerateQR();
        Initilise();
    }

    private void Initilise()
    {
        SelectedDates = ZilePlatiteKeys;
        RemoveDays();
        ForceCalendarRefresh?.Invoke();


    }



    private void GenerateQR()
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(_user.Id.ToString(), QRCodeGenerator.ECCLevel.L);
        PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);

        QrCodeImage = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }

    private void RemoveDays()
    {
        DateTime dataInceput = _user.ZilePlatite.Keys.Min();
        DateTime dataFinal = _user.ZilePlatite.Keys.Max();

        while (dataInceput.Date != dataFinal.Date.AddDays(1))
        {
          
            try
            {
                var dataGasita = _user.ZilePlatite.First(d => d.Key.Date == dataInceput.Date);
                if (dataGasita.Value == 1||dataInceput.Date<DateTime.Now.Date)
                {
                    _events[dataInceput] = new DayEventCollection<EventModel>(Colors.PaleTurquoise, Colors.PaleTurquoise)
            {
                new() { Name = "", Description = "", StartTime= dataInceput}
            };
                }
                
            }
            catch
            {
                _events[dataInceput] = new DayEventCollection<EventModel>(Colors.White, Colors.White)
            {
                new() { Name = "", Description = "", StartTime= dataInceput}
            };
            }  
            dataInceput = dataInceput.AddDays(1);
        }
        OnPropertyChanged(nameof(Events));
    }
  

    private async void OnDayTapped(DateTime dateTapped)
    {

        try
        {
            if (dateTapped.Date >= DateTime.Now.Date && dateTapped.DayOfWeek != DayOfWeek.Saturday && dateTapped.DayOfWeek != DayOfWeek.Sunday)
            {

                if (_user.ZilePlatite.Where(d => d.Key.Date == dateTapped.Date).First().Value == 0)
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmare", "Adaugă restanță?", "Da", "Nu");

                    if (confirm)
                    {
                        if (DateTime.Now.Date == dateTapped.Date && DateTime.Now.Hour >= 8)
                        {
                            await Application.Current.MainPage.DisplayAlert("Alerta", "Nu puteti adauga restanta dupa ora 8", "Cancel");
                        }

                        else
                        {
                            FirestoreService.UpdateZilePlatite(dateTapped, _user, 1);

                            if (!_events.ContainsKey(dateTapped))
                            {
                                _events[dateTapped] = new DayEventCollection<EventModel>(Colors.AliceBlue, Colors.AliceBlue)
                {
        new() { Name = "", Description = "", StartTime = dateTapped }
                  };
                            }

                            _events[dateTapped] = new DayEventCollection<EventModel>(Colors.AliceBlue, Colors.AliceBlue)
            {
                new() { Name = "", Description = "", StartTime= dateTapped}
            };
                            _user.ZilePlatite[dateTapped] = 1;
                        }

                    }


                }
                else
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmare", "Anuleaza restanță?", "Da", "Nu");
                    if (confirm)
                    {
                        if (DateTime.Now.Date == dateTapped.Date && DateTime.Now.Hour > 8)
                        {
                            await Application.Current.MainPage.DisplayAlert("Alerta", "Nu puteti anula restanta dupa ora 8", "Cancel");

                        }
                        else
                        {
                            if (Events.ContainsKey(dateTapped))
                            {
                                Events.Remove(dateTapped);
                            }
                            _user.ZilePlatite[dateTapped] = 0;
                            FirestoreService.UpdateZilePlatite(dateTapped, _user, 0);


                        }


                    }


                }

            }
        }
        catch
        {
        }

        SelectedDates = new List<DateTime>(_user.ZilePlatite.Keys);
        OnPropertyChanged(nameof(SelectedDates));
        ForceCalendarRefresh?.Invoke();
        OnPropertyChanged(nameof(Events));


    }



    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
