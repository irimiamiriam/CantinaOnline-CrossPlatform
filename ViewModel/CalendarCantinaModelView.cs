using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CantinaOnline.Models;

public class CalendarCantinaModelView : INotifyPropertyChanged
{
    private List<DateTime> _selectedDates;
    private List<DateTime> _zileRestante;
    private Dictionary<string, int> _zileCantina;
    private List<DateTime> _tempDates;

    public event PropertyChangedEventHandler PropertyChanged;

    public List<DateTime> SelectedDates
    {
        get => _selectedDates;
        set
        {
            _selectedDates = value;
            OnPropertyChanged();
        }
    }

    public List<DateTime> ZileRestante
    {
        get => _zileRestante;
        set
        {
            _zileRestante = value;
            OnPropertyChanged();
        }
    }

    public ICommand DayTappedCommand { get; }
    public ICommand UpdateCalendarCommand { get; }

    public CalendarCantinaModelView()
    {
        _selectedDates = new List<DateTime>();
        _zileRestante = new List<DateTime>();

        DayTappedCommand = new RelayCommand<DateTime>(OnDayTapped);
        UpdateCalendarCommand = new RelayCommand(async () => await UpdateCalendar());

        InitializeCalendar();
    }

    private async void InitializeCalendar()
    {
        await LoadSelectedDates();
    }

    private async Task LoadSelectedDates()
    {
        _zileCantina = await FirestoreService.GetAdminZileCantina();

        _tempDates = new List<DateTime>();

        foreach (var dateString in _zileCantina.Keys)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                _tempDates.Add(date);
            }
        }

        SelectedDates = _tempDates;

        // After loading selected dates, initialize zile restante
        ZileRestante = SelectedDates
            .Where(d => d.Date < DateTime.Now.Date) // before today
            .ToList();
    }

    private void OnDayTapped(DateTime date)
    {
        if (SelectedDates.Any(d => d.Date == date.Date))
        {
            SelectedDates.RemoveAll(d => d.Date == date.Date);
        }
        else
        {
            SelectedDates.Add(date);
        }

        // Recalculate zile restante
        ZileRestante = SelectedDates
            .Where(d => d.Date < DateTime.Now.Date)
            .ToList();

        OnPropertyChanged(nameof(SelectedDates));
        OnPropertyChanged(nameof(ZileRestante));
    }

    private async Task UpdateCalendar()
    {
        Dictionary<string, int> zileCantinaToSave = new();

        foreach (var date in SelectedDates)
        {
            zileCantinaToSave[date.ToString("yyyy-MM-dd")] = 0; // 0 = normal available day
        }

        await FirestoreService.UpdateAdminZileCantina(zileCantinaToSave);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

