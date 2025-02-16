using CantinaOnline.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

public class FirestoreService
{
    private FirestoreDb _db;
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
            Console.WriteLine($"❌ Firestore initialization failed: {ex.Message}");
            Console.WriteLine($"💡 Inner Exception: {ex.InnerException?.Message}");
            throw;
        }
    }

    public bool CheckConnection()
    {
        return IsConnected;
    }
    public async Task<ElevModel> GetElevByPassword(string password)
    {
        try
        {
            CollectionReference usersRef = _db.Collection("Users");
            Query query = usersRef.WhereEqualTo("Parola", password);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                var doc = snapshot.Documents[0];
                return new ElevModel
                {
                    Id = Convert.ToInt32(doc.Id),
                    Nume = doc.GetValue<string>("Nume"),
                    ZilePlatite = doc.ContainsField("ZilePlatite") ? doc.GetValue<List<DateTime>>("ZilePlatite") : new List<DateTime>()
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving user: {ex.Message}");
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

}

