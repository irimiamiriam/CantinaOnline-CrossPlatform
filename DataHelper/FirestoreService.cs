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
            string localPath = Path.Combine(FileSystem.AppDataDirectory, "cantinacrossplatform-firebase-adminsdk-fbsvc-2bb69a01f9.json"); // Place this in secure storage, not directly in code.
            string credentialsPath = localPath;
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
  

   
}
