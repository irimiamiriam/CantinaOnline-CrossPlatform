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
            await SyncZilePlatiteWithAdmin(userId);
        });
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
