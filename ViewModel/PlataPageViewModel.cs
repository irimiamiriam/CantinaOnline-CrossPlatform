using CantinaOnline.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

public class PlataPageViewModel : ObservableObject
{
    private string _selectedClass;
    public string SelectedClass

    {
        get => _selectedClass;
        set
        {
            SetProperty(ref _selectedClass, value);
            LoadStudents(); // Load students when a class is selected
        }
    }
    public IRelayCommand PayForClassCommand { get; }

    public ObservableCollection<string> Classes { get; } = new()
    {
        "9A", "9B", "9C", "9D", "9E", "9F", "9G",
        "10A", "10B", "10C", "10D", "10E", "10F", "10G",
        "11A", "11B", "11C", "11D", "11E", "11F", "11G",
        "12A", "12B", "12C", "12D", "12E", "12F", "12G"
    };

    public ObservableCollection<ElevModel> Students { get; } = new();

    public IRelayCommand<int> AddToZilePlatiteCommand { get; }

    public PlataPageViewModel()
    {
        AddToZilePlatiteCommand = new RelayCommand<int>(async (userId) => {
            await ShowConfirmationDialog(userId);
        });
        PayForClassCommand = new RelayCommand(async () =>
        {
            await PayForWholeClassAsync();
        });
    }

    private async Task PayForWholeClassAsync()
    {
        if (Students.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Eroare", "Nu există elevi în clasă.", "OK");
            return;
        }

        var adminZileCantina = await FirestoreService.GetAdminZileCantina();
        if (adminZileCantina == null || adminZileCantina.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Eroare", "Zilele cantinei nu sunt disponibile.", "OK");
            return;
        }

        int totalZile = adminZileCantina.Count;
        int totalRestante = 0;

        foreach (var student in Students)
        {
            totalRestante += student.ZilePlatite.Count(z => z.Value == 1);
        }

        int totalCost = (totalZile * Students.Count - totalRestante) * 18;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirmare plată",
            $"Număr elevi: {Students.Count}\nTotal restante: {totalRestante}\nTotal de plată: {totalCost} RON\nContinuați?",
            "Da", "Nu");

        if (confirm)
        {
            foreach (var student in Students)
            {
                await FirestoreService.UpdateUserZilePlatite(student.Id, adminZileCantina);
            }

            await Application.Current.MainPage.DisplayAlert("Succes", "Zilele plătite au fost actualizate pentru toată clasa.", "OK");

            LoadStudents(); // Refresh data
        }
    }



    private async Task ShowConfirmationDialog(int userId)
    {
        var user = await FirestoreService.GetUserById(userId.ToString());
        if (user == null) return;

        // Get admin's ZileCantina
        Dictionary<string, int> adminZileCantina = await FirestoreService.GetAdminZileCantina();
        if (adminZileCantina == null || adminZileCantina.Count == 0) return;

        // Calculate remaining days (Restante)
        int restante = user.ZilePlatite.Values.Count(v => v == 1);
        int totalCost = adminZileCantina.Count * 18;

        // Show confirmation popup
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirmare",
            $"Restante: {restante} zile\nTotal: {totalCost-restante*18} RON\nDoriți să actualizați zilele plătite?",
            "OK",
            "Anulează"
        );

        if (confirm)
        {
            await SyncZilePlatiteWithAdmin(user.Id);
        }
    }

    private async void LoadStudents()
    {
        if (string.IsNullOrEmpty(SelectedClass)) return;

        Students.Clear();
        var students = await FirestoreService.GetUsersByClass(SelectedClass);
        foreach (var student in students)
        {
            Students.Add(student);
        }
    }

    private async Task SyncZilePlatiteWithAdmin(int userId)
    {
        var user = await FirestoreService.GetUserById(userId.ToString());
        if (user == null) return;

        // Get admin's ZileCantina
        Dictionary<string, int> adminZileCantina = await FirestoreService.GetAdminZileCantina();
        if (adminZileCantina == null || adminZileCantina.Count == 0) return;

        // Replace student's ZilePlatite with admin's ZileCantina
        await FirestoreService.UpdateUserZilePlatite(user.Id, adminZileCantina);

        // Refresh the list to show updated data
        LoadStudents();
    }
}
