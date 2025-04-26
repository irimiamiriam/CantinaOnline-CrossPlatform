using CantinaOnline.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

public class ElevNouViewModel : ObservableObject
{
    private string _numeElev;
    public string NumeElev
    {
        get => _numeElev;
        set => SetProperty(ref _numeElev, value);
    }

    private string _selectedClass;
    public string SelectedClass
    {
        get => _selectedClass;
        set => SetProperty(ref _selectedClass, value);
    }

    private string _generatedPassword;
    public string GeneratedPassword
    {
        get => _generatedPassword;
        set => SetProperty(ref _generatedPassword, value);
    }

    private string _generatedId;
    public string GeneratedId
    {
        get => _generatedId;
        set => SetProperty(ref _generatedId, value);
    }

    public ObservableCollection<string> Classes { get; } = new()
    {
        "9A", "9B", "9C", "9D", "9E", "9F", "9G",
        "10A", "10B", "10C", "10D", "10E", "10F", "10G",
        "11A", "11B", "11C", "11D", "11E", "11F", "11G",
        "12A", "12B", "12C", "12D", "12E", "12F", "12G"
    };

    public IRelayCommand GenerateCredentialsCommand { get; }
    public IRelayCommand AddElevCommand { get; }

    public ElevNouViewModel()
    {
        GenerateCredentialsCommand = new RelayCommand(GenerateCredentials);
        AddElevCommand = new AsyncRelayCommand(AddElev);
    }

    private void GenerateCredentials()
    {
        Random random = new();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        GeneratedPassword = new string(Enumerable.Range(0, 8)
                                        .Select(_ => chars[random.Next(chars.Length)]) 
                                        .ToArray());

        GeneratedId = random.Next(10000, 99999).ToString(); 
    }

    private async Task AddElev()
    {
        if (string.IsNullOrWhiteSpace(NumeElev) || string.IsNullOrWhiteSpace(SelectedClass) ||
            string.IsNullOrWhiteSpace(GeneratedPassword) || string.IsNullOrWhiteSpace(GeneratedId))
        {
            await Application.Current.MainPage.DisplayAlert("Eroare", "Completează toate câmpurile!", "OK");
            return;
        }

        // Get admin's ZileCantina
        Dictionary<string, int> zileCantina = await FirestoreService.GetAdminZileCantina();
        if (zileCantina == null || zileCantina.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Eroare", "Nu există ZileCantina!", "OK");
            return;
        }

        // Create the new student object
        ElevModel newElev = new ElevModel
        {
            Id = int.Parse(GeneratedId),
            Nume = NumeElev,
            Clasa = SelectedClass,
            Parola = GeneratedPassword,
            LastScan = ""
        };

        await FirestoreService.AddUser(newElev, zileCantina);

        await Application.Current.MainPage.DisplayAlert("Succes", "Elev adăugat cu succes!", "OK");

        // Reset fields after adding
        NumeElev = "";
        SelectedClass = null;
        GeneratedPassword = "";
        GeneratedId = "";
    }
}
