using Microsoft.UI.Xaml;

namespace PIA_PrograLocal
{
    public partial class App : Application
    {
        // Siempre existirá después de OnLaunched → por eso usamos null!
        public static Window MainAppWindow { get; private set; } = null!;

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Crear la ventana principal de la app
            MainAppWindow = new MainWindow();
            MainAppWindow.Activate();
        }

        /// <summary>
        /// Cambia la ventana principal por otra. Úsalo solo si realmente
        /// quieres cerrar la ventana actual y abrir una nueva.
        /// </summary>
        public static void CambiarVentana(Window nuevaVentana)
        {
            // Cerrar la antigua ventana si existe
            MainAppWindow?.Close();

            // Establecer la nueva
            MainAppWindow = nuevaVentana;
            MainAppWindow.Activate();
        }
    }
}
