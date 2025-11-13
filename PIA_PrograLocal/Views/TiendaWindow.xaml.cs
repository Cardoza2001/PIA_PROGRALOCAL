using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PIA_PrograLocal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PIA_PrograLocal.Helpers;

namespace PIA_PrograLocal.Views
{
    public sealed partial class TiendaWindow : Page
    {
        private readonly Usuario usuarioActual;
        private readonly List<Auto> carrito = new();
        private Auto? autoSeleccionado;

        public TiendaWindow()
        {
            this.InitializeComponent();

            var currentWindow = WindowHelper.GetWindowForElement(this);
            if (currentWindow != null)
                WindowHelper.RegisterWindow(this, currentWindow);

            usuarioActual = UsuarioManager.UsuarioLogueado ?? new Usuario
            {
                Nombre = "Invitado",
                Correo = "sincorreo@ejemplo.com"
            };

            AutosData.Inicializar();
            MarcaComboBox.ItemsSource = AutosData.Autos
                .Select(a => a.Marca)
                .Distinct()
                .ToList();
        }

        private void MarcaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarcaComboBox.SelectedItem is string marca)
            {
                var modelos = AutosData.Autos.Where(a => a.Marca == marca).ToList();
                ModelosListView.ItemsSource = modelos;
                SeleccionAñoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ModelosListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelosListView.SelectedItem is Auto auto)
            {
                autoSeleccionado = auto;
                AnioComboBox.ItemsSource = new List<int> { 2022, 2023, 2024, 2025 };
                AnioComboBox.SelectedIndex = 0;
                SeleccionAñoPanel.Visibility = Visibility.Visible;
            }
        }

        private async void AgregarCarritoButton_Click(object sender, RoutedEventArgs e)
        {
            if (autoSeleccionado != null && AnioComboBox.SelectedItem is int anio)
            {
                carrito.Add(new Auto
                {
                    Marca = autoSeleccionado.Marca,
                    Modelo = $"{autoSeleccionado.Modelo} ({anio})",
                    Descripcion = autoSeleccionado.Descripcion,
                    Precio = autoSeleccionado.Precio
                });

                CarritoListView.ItemsSource = null;
                CarritoListView.ItemsSource = carrito;
                ActualizarTotal();

                var dialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Agregado al carrito",
                    Content = $"{autoSeleccionado.Marca} {autoSeleccionado.Modelo} ({anio}) fue agregado correctamente.",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();

                MarcaComboBox.SelectedIndex = -1;
                ModelosListView.ItemsSource = null;
                SeleccionAñoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void EliminarDelCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Auto auto)
            {
                carrito.Remove(auto);
                CarritoListView.ItemsSource = null;
                CarritoListView.ItemsSource = carrito;
                ActualizarTotal();
            }
        }

        private async void VerCarritoButton_Click(object sender, RoutedEventArgs e)
        {
            if (carrito.Count == 0)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Carrito vacío",
                    Content = "Agrega al menos un vehículo antes de finalizar la compra.",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
                return;
            }

            double total = carrito.Sum(a => a.Precio);
            string resumen = string.Join("\n", carrito.Select(a =>
                $"{a.Marca} {a.Modelo}\n{a.Descripcion}\nPrecio: {a.Precio:C}\n"));

            var confirmDialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Confirmar compra",
                Content = $"{resumen}\nTOTAL: {total:C}\n\n¿Deseas generar el ticket en PDF?",
                PrimaryButtonText = "Sí",
                CloseButtonText = "No"
            };

            var result = await confirmDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string folio = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                string rutaPDF = GenerarTicketPDF(folio);

                var historial = new HistorialCompra
                {
                    Usuario = usuarioActual?.Nombre ?? "Invitado",
                    Folio = folio,
                    Autos = new List<Auto>(carrito),
                    RutaPDF = rutaPDF,
                    Fecha = DateTime.Now,
                    Total = total
                };

                HistorialData.AgregarCompra(historial);

                var doneDialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Compra realizada",
                    Content = $"Se ha generado el ticket en tu escritorio.\nFolio: {folio}",
                    CloseButtonText = "OK"
                };
                await doneDialog.ShowAsync();

                carrito.Clear();
                CarritoListView.ItemsSource = null;
                ActualizarTotal();
            }
        }

        private string GenerarTicketPDF(string folio)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{usuarioActual!.Nombre}_Ticket_{folio}.pdf";
            string filePath = Path.Combine(desktopPath, fileName);

            var pdf = new PdfDocument();
            var page = pdf.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            double y = 50;

            gfx.DrawString("TICKET DE COMPRA - Tienda de Autos", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            y += 40;
            gfx.DrawString($"Folio: {folio}", new XFont("Arial", 11, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            y += 20;
            gfx.DrawString($"Cliente: {usuarioActual.Nombre}", new XFont("Arial", 12), XBrushes.Black, new XPoint(50, y));
            y += 20;
            gfx.DrawString($"Correo: {usuarioActual.Correo}", new XFont("Arial", 11), XBrushes.Black, new XPoint(50, y));
            y += 40;

            foreach (var auto in carrito)
            {
                gfx.DrawString($"{auto.Marca} - {auto.Modelo}", new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
                y += 20;
                gfx.DrawString(auto.Descripcion, new XFont("Arial", 11), XBrushes.Gray, new XPoint(50, y));
                y += 20;
                gfx.DrawString($"Precio: {auto.Precio:C}", new XFont("Arial", 11), XBrushes.Black, new XPoint(50, y));
                y += 30;
            }

            gfx.DrawString($"TOTAL: {carrito.Sum(a => a.Precio):C}", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            pdf.Save(filePath);
            return filePath;
        }

        private void ActualizarTotal()
        {
            double total = carrito.Sum(a => a.Precio);
            System.Diagnostics.Debug.WriteLine($"TOTAL: {total:C}");
        }

        private async void CerrarSesionButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Cerrar sesión",
                Content = "¿Seguro que deseas cerrar sesión?",
                PrimaryButtonText = "Sí",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                UsuarioManager.UsuarioLogueado = null;

                var mainWindow = new MainWindow();
                App.CambiarVentana(mainWindow);
            }
        }

        private void HistorialButton_Click(object sender, RoutedEventArgs e)
        {
            var historialWindow = new Microsoft.UI.Xaml.Window
            {
                Content = new HistorialWindow(usuarioActual)
            };
            historialWindow.Activate();
        }
    }
}
