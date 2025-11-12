using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PIA_PrograLocal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PIA_PrograLocal.Views
{
    public sealed partial class TiendaWindow : Page
    {
        private readonly Usuario usuarioActual;
        private List<Auto> carrito = new();
        private Auto? autoSeleccionado;

        public TiendaWindow()
        {
            this.InitializeComponent();
            usuarioActual = UsuarioManager.UsuarioLogueado ?? new Usuario { Nombre = "Invitado" };
            AutosData.Inicializar();
            MarcaComboBox.ItemsSource = AutosData.Autos.Select(a => a.Marca).Distinct().ToList();
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
                GenerarTicketPDF();
                var doneDialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Compra realizada",
                    Content = "Se ha generado el ticket en tu escritorio.",
                    CloseButtonText = "OK"
                };
                await doneDialog.ShowAsync();
                carrito.Clear();
            }
        }

        private void GenerarTicketPDF()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string filePath = Path.Combine(desktopPath, "Ticket_Compra.pdf");

            var pdf = new PdfDocument();
            var page = pdf.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            double y = 50;

            gfx.DrawString("TICKET DE COMPRA - Tienda de Autos", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
            y += 40;

            gfx.DrawString($"Cliente: {usuarioActual.Nombre}", new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(50, y));
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
        }
    }
}
