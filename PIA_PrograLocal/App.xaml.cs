using Microsoft.UI.Xaml;

namespace PIA_PrograLocal
{
    public partial class App : Application
    {
        public static Window MainAppWindow { get; private set; } = null!;

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainAppWindow = new MainWindow();
            MainAppWindow.Activate();
        }

        /// <summary>
        /// Cambia la ventana principal por otra. Úsalo solo si realmente
        /// quieres cerrar la ventana actual y abrir una nueva.
        /// </summary>
        public static void CambiarVentana(Window nuevaVentana)
        {
            MainAppWindow?.Close();
            MainAppWindow = nuevaVentana;
            MainAppWindow.Activate();
        }
    }
}
