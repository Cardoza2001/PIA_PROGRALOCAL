using Microsoft.UI.Xaml;

namespace PIA_PrograLocal
{
    public partial class App : Application
    {
        public static Window? MainAppWindow { get; set; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainAppWindow = new MainWindow();
            MainAppWindow.Activate();
        }

        public static void CambiarVentana(Window nuevaVentana)
        {
            // Cierra la actual si existe
            MainAppWindow?.Close();
            MainAppWindow = nuevaVentana;
            MainAppWindow.Activate();
        }
    }
}
