using CommunityToolkit.Maui;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Camera.MAUI;
namespace CantinaOnline
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

        

            builder.Services.AddSingleton<FirestoreService>();


            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .UseMauiCommunityToolkit()
                            

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Rony.ttf", "Rony");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();


        }
    }
}
