using CantinaOnline.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

public class FirestoreService
{
    private static FirestoreDb _db;
    public bool IsConnected { get; private set; } = false;

    public FirestoreService()
    {
        try
        {
            string projectId = "cantinacrossplatform";

            string credentialsPath = EnsureCredentialsFileExistsAsync().Result;





            _db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = GoogleCredential.FromFile(credentialsPath)
            }.Build();
            IsConnected = true;
        }

        catch (Exception ex)
        {
            Console.WriteLine($" Firestore initialization failed: {ex.Message}");
            Console.WriteLine($" Inner Exception: {ex.InnerException?.Message}");
            throw;
        }
    }

    public bool CheckConnection()
    {
        return IsConnected;
    }
    public async Task<ElevModel?> GetElevByPassword(string password)
    {
        try
        {
            CollectionReference usersRef = _db.Collection("Users");
            Query query = usersRef.WhereEqualTo("Parola", password);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            int i = 0;
            if (snapshot.Documents.Count > 0)
            {
                var doc = snapshot.Documents[0];

                // Fetch ZilePlatite as a Dictionary<string, int> from Firestore
                Dictionary<string, int> zilePlatiteMap = doc.ContainsField("ZilePlatite")
                    ? doc.GetValue<Dictionary<string, int>>("ZilePlatite")
                    : new Dictionary<string, int>();

                // Convert to Dictionary<DateTime, int>
                Dictionary<DateTime, int> zilePlatite = zilePlatiteMap
                    .ToDictionary(
                        kvp => DateTime.Parse(kvp.Key).Date, // Convert string key to DateTime
                        kvp => kvp.Value // Keep int value (0 or 1)
                    );

                return new ElevModel
                {
                    Id = Convert.ToInt32(doc.Id),
                    Nume = doc.GetValue<string>("Nume"),
                    ZilePlatite = zilePlatite
                };
            } 
            else
                return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error retrieving user: {ex.Message}");
            return null;
      
        }
    }

    public async Task<string> EnsureCredentialsFileExistsAsync()
    {
        string targetPath = Path.Combine(FileSystem.AppDataDirectory, "cantinacrossplatform-firebase-adminsdk-fbsvc-2bb69a01f9.json");


        if (!File.Exists(targetPath))
        {
            using (var stream = typeof(FirestoreService).Assembly.GetManifestResourceStream("CantinaOnline.Resources.Raw.cantinacrossplatform-firebase-adminsdk-fbsvc-2bb69a01f9.json"))
            {
                if (stream == null)
                    throw new InvalidOperationException("Unable to find the embedded Firebase credentials file.");

                using (var fileStream = File.Create(targetPath))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }

        return targetPath; 
    }

    internal async Task<AdminModel> GetAdminByPassword(string password)
    {

        try
        {
            CollectionReference usersRef = _db.Collection("Admins");
            Query query = usersRef.WhereEqualTo("Parola", password);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                var doc = snapshot.Documents[0];
                return new AdminModel
                {
                    
                    Rol= doc.GetValue<string>("Rol")
                  
                };
            }
            else
                return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error retrieving user: {ex.Message}");
            return null;

        }
    }


    //public static async Task UpdateUserZilePlatite()
    //{
    //    try
    //    {



    //        CollectionReference usersRef = _db.Collection("Users");
    //        Query query = usersRef.WhereEqualTo("Parola", "miriam100");
    //        QuerySnapshot snapshot = await query.GetSnapshotAsync();



    //        DocumentReference userDoc = snapshot.Documents[0].Reference;

    //        // Generate all weekdays for Martie
    //        Dictionary<string, int> zilePlatite = new Dictionary<string, int>();
    //        int year = DateTime.Now.Year; 
    //        int month = 3;

    //        int daysInMonth = DateTime.DaysInMonth(year, month);
    //        for (int day = 1; day <= daysInMonth; day++)
    //        {
    //            DateTime date = new DateTime(year, month, day);

    //            // Skip Sambt&Dum
    //            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
    //            {
    //                zilePlatite[date.ToString("dd-MM-yyyy")] = 0;
    //            }
    //        }

    //        // Update Firestore document
    //        await userDoc.UpdateAsync(new Dictionary<string, object>
    //    {
    //        { "ZilePlatite", zilePlatite }
    //    });


    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($" Error updating user: {ex.Message}");
    //    }
    //}
    public static async Task UpdateZilePlatite(DateTime selectedDate, ElevModel user, int value)
    {
        try
        {

            // Get the user document
            DocumentReference userDoc = _db.Collection("Users").Document(user.Id.ToString());
            DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

           

            // Get existing data
            Dictionary<string, object> userData = snapshot.ToDictionary();
            Dictionary<string, int> zilePlatite = userData.ContainsKey("ZilePlatite")
                ? snapshot.GetValue<Dictionary<string, int>>("ZilePlatite")
                : new Dictionary<string, int>();

            int nrRestante = userData.ContainsKey("NrRestante") ? snapshot.GetValue<int>("NrRestante") : 0;

            // Mark the selected date as 1 (Restanță)
            zilePlatite[selectedDate.ToString("yyyy-MM-dd")] = value;

            // Increment NrRestante
            nrRestante++;

            // Update Firestore
            await userDoc.UpdateAsync(new Dictionary<string, object>
        {
            { "ZilePlatite", zilePlatite },
            { "NrRestante", nrRestante }
        });

        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error updating Firestore: {ex.Message}");
        }
    
    }

    public static async Task<int> GetUsersEatingToday()
    {
        try
        {
            CollectionReference usersRef = _db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            DateTime today = DateTime.Today;
            string todayString = today.ToString("yyyy-MM-dd");

            int count = snapshot.Documents.Count(doc =>
                doc.ContainsField("ZilePlatite") &&
                doc.GetValue<Dictionary<string, int>>("ZilePlatite")
                    .ContainsKey(todayString) &&
                doc.GetValue<Dictionary<string, int>>("ZilePlatite")[todayString] == 0);

            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving users eating today: {ex.Message}");
            return 0; // Return 0 if an error occurs
        }
    }


}

