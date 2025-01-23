using Microsoft.Extensions.Logging;

namespace RandomStudentGenerator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Gabarito-VariableFont_wght.ttf", "Gabarito")
                        .AddFont("OpenSans-Regular.ttf", "OpenSans-Regular")
                        .AddFont("OpenSans-SemiBold.ttf", "OpenSans-SemiBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
