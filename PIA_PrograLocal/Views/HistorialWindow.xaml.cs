using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PIA_PrograLocal.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PIA_PrograLocal.Views
{
    public sealed partial class HistorialWindow : Page
    {
        private readonly Usuario _usuarioActual;

        public HistorialWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuarioActual = usuario ?? throw new ArgumentNullException(nameof(usuario));

            // Usa la propiedad "Nombre" en lugar de "NombreUsuario"
            CargarHistorial(_usuarioActual.Nombre ?? "Invitado");
        }

        private void CargarHistorial(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return;

            // Carga el historial directamente en la colección compartida
            HistorialData.CargarHistorial(nombreUsuario);
            ListViewHistorial.ItemsSource = HistorialData.Compras;
        }

        private async void BtnVerTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HistorialCompra compra)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(compra.RutaPDF) && File.Exists(compra.RutaPDF))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = compra.RutaPDF,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        await MostrarDialogo("Error", "No se encontró el ticket PDF.");
                    }
                }
                catch (Exception ex)
                {
                    await MostrarDialogo("Error", $"Error al abrir el ticket: {ex.Message}");
                }
            }
        }

        private async Task MostrarDialogo(string titulo, string mensaje)
        {
            var dialog = new ContentDialog
            {
                Title = titulo,
                Content = mensaje,
                CloseButtonText = "Aceptar",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
