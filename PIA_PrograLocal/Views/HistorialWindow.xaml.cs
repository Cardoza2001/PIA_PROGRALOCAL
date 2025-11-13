using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PIA_PrograLocal.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace PIA_PrograLocal.Views
{
    public sealed partial class HistorialWindow : Page
    {
        private readonly Usuario _usuarioActual;

        public HistorialWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuarioActual = usuario ?? throw new ArgumentNullException(nameof(usuario));
            CargarHistorial(_usuarioActual.Nombre ?? "Invitado");
        }

        private void CargarHistorial(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return;

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
                        await MostrarDialogo("Ticket no encontrado",
                            "No se encontró el archivo PDF. Puedes regenerarlo.");
                    }
                }
                catch (Exception ex)
                {
                    await MostrarDialogo("Error", $"Error al abrir el ticket: {ex.Message}");
                }
            }
        }

        private async void BtnRegenerarPDF_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HistorialCompra compra)
            {
                try
                {
                    string rutaPDF = GenerarNuevoTicketPDF(compra);
                    compra.RutaPDF = rutaPDF;

                    await MostrarDialogo("Éxito", $"Ticket regenerado en tu escritorio:\n{Path.GetFileName(rutaPDF)}");
                }
                catch (Exception ex)
                {
                    await MostrarDialogo("Error", $"No se pudo regenerar el ticket: {ex.Message}");
                }
            }
        }

        private string GenerarNuevoTicketPDF(HistorialCompra compra)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{_usuarioActual.Nombre}_Ticket_{compra.Folio}_Regenerado.pdf";
            string filePath = Path.Combine(desktopPath, fileName);

            var pdf = new PdfDocument();
            var page = pdf.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            double y = 50;

            gfx.DrawString("TICKET DE COMPRA - Tienda de Autos", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            y += 40;
            gfx.DrawString($"Folio: {compra.Folio}", new XFont("Arial", 11, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            y += 20;
            gfx.DrawString($"Cliente: {_usuarioActual.Nombre}", new XFont("Arial", 12), XBrushes.Black, new XPoint(50, y));
            y += 20;
            gfx.DrawString($"Correo: {_usuarioActual.Correo}", new XFont("Arial", 11), XBrushes.Black, new XPoint(50, y));
            y += 30;

            foreach (var auto in compra.Autos)
            {
                gfx.DrawString($"{auto.Marca} - {auto.Modelo}", new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
                y += 20;
                gfx.DrawString(auto.Descripcion, new XFont("Arial", 11), XBrushes.Gray, new XPoint(50, y));
                y += 20;
                gfx.DrawString($"Precio: {auto.Precio:C}", new XFont("Arial", 11), XBrushes.Black, new XPoint(50, y));
                y += 30;
            }

            gfx.DrawString($"TOTAL: {compra.Total:C}", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            pdf.Save(filePath);
            return filePath;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            var nuevaVentana = new Microsoft.UI.Xaml.Window
            {
                Content = new TiendaWindow(_usuarioActual)
            };

            nuevaVentana.Activate();

            var ventanaActual = Microsoft.UI.Xaml.Window.Current;
            ventanaActual.Close();
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
