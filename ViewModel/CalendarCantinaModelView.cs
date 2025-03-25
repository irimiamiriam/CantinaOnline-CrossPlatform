using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

public class CalendarCantinaModelView : ObservableObject
{
    private readonly FirestoreService _firestoreService;

    Dictionary<string, int> zileCantina = new Dictionary<string, int>();

    private ObservableCollection<DateTime> _selectedDates;
    public ObservableCollection<DateTime> SelectedDates
    {
        get => _selectedDates;
        set => SetProperty(ref _selectedDates, value);
    }

    private Dictionary<DateTime, string> _events;
    public Dictionary<DateTime, string> Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }

    public IRelayCommand<DateTime> DayTappedCommand { get; }

    public CalendarCantinaModelView(FirestoreService firestoreService)
    {
        _firestoreService = firestoreService;

        SelectedDates = new ObservableCollection<DateTime>();
        Events = new Dictionary<DateTime, string>();

        DayTappedCommand = new RelayCommand<DateTime>(ToggleEventOnDate);

        InitializeCalendar();
    }

    private async void InitializeCalendar()
    {
        await LoadZileCantinaFromDatabase();
        MarkWeekendsAsEvents();
    }

    private async Task LoadZileCantinaFromDatabase()
    {
        zileCantina = await FirestoreService.GetAdminZileCantina();

        if (zileCantina != null)
        {
            SelectedDates.Clear();
            foreach (var dateString in zileCantina.Keys)
            {
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    SelectedDates.Add(date);
                }
            }
        }
    }

    private void MarkWeekendsAsEvents()
    {
        DateTime firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        for (int i = 1; i <= daysInMonth; i++)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);

            if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday || !zileCantina.Keys.Contains(currentDate.Date.ToString()))
            {
                Events[currentDate] = "";
            }
        }
    }

    private void ToggleEventOnDate(DateTime date)
    {
        if (Events.ContainsKey(date))
        {
            Events.Remove(date); 
        }
        else
        {
            Events[date] = ""; 
        }
    }

    public async Task UpdateZileCantinaInDatabase()
    {
        Dictionary<string, int> zileCantinaToSave = new();

        foreach (var date in SelectedDates)
        {
            if (!Events.ContainsKey(date)) 
            {
                zileCantinaToSave[date.ToString("yyyy-MM-dd")] = 0;
            }
        }

        await _firestoreService.UpdateAdminZileCantina(zileCantinaToSave);
    }
}
