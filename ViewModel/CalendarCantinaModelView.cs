using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CantinaOnline.Models;
using Plugin.Maui.Calendar.Models;
using CommunityToolkit.Mvvm.Input;

public class CalendarCantinaModelView : INotifyPropertyChanged
{
    private EventCollection _events;
    private List<DateTime> _selectedDates;
    private FirestoreService _firestoreService;

    public event PropertyChangedEventHandler PropertyChanged;

    public EventCollection Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }

    public List<DateTime> SelectedDates
    {
        get => _selectedDates;
        set
        {
            _selectedDates = value;
            OnPropertyChanged();
        }
    }
    List<DateTime> tempDates;
    Dictionary<string, int> zileCantina;
    public ICommand DayTappedCommand { get; }
    public ICommand UpdateCalendarCommand { get; }

    public CalendarCantinaModelView()
    {
        _firestoreService = new FirestoreService();
        _events = new EventCollection();
        _selectedDates = new List<DateTime>();

        DayTappedCommand = new RelayCommand<DateTime>(OnDayTapped);
        UpdateCalendarCommand = new RelayCommand(async () => await UpdateCalendar());

        InitializeCalendar();
    }

    private async void InitializeCalendar()
    {
        await LoadSelectedDates();
        MarkWeekendsAsEvents();
    }

    private async Task LoadSelectedDates()
    {
        zileCantina = await FirestoreService.GetAdminZileCantina();

        if (zileCantina != null)
        {
            tempDates = new List<DateTime>();

            foreach (var dateString in zileCantina.Keys)
            {
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    tempDates.Add(date);
                }
            }

            SelectedDates = tempDates;
            Console.WriteLine($"Loaded {SelectedDates.Count} selected dates from Firestore.");
        }


        // Refresh UI
        OnPropertyChanged(nameof(SelectedDates));
    }

    private void MarkWeekendsAsEvents()
    {
        DateTime firstDay = DateTime.Parse(zileCantina.Keys.Min());
        DateTime lastDay = DateTime.Parse(zileCantina.Keys.Max());

        for (DateTime date = firstDay; date <= lastDay; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || !zileCantina.ContainsKey(date.ToString("yyyy-MM-dd")))
            {
                _events[date] = new List<EventModel> { new EventModel { Name = "", Description = "" } };
            }
        }

        // Refresh UI
        OnPropertyChanged(nameof(Events));
    }

    private async void OnDayTapped(DateTime date)
    {
        if (Events.ContainsKey(date))
        {
            Events.Remove(date);  // Remove event if it was already added
        }
        else
        {
            Events[date] = new List<EventModel> { new EventModel { Name = "", Description = "" } }; // Add event
        }
        SelectedDates = tempDates;
    }
    

    private async Task UpdateCalendar()
    {
        Dictionary<string, int> zileCantinaToSave = new();

        foreach (var date in SelectedDates)
        {
            if (!Events.ContainsKey(date))
            {
                zileCantinaToSave[date.ToString("yyyy-MM-dd")] = 0;
            }
        }

        await FirestoreService.UpdateAdminZileCantina(zileCantinaToSave);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

