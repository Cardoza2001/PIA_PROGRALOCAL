using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PIA_PrograLocal.Models
{
    public static class HistorialData
    {
        private static readonly string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static readonly string historialPath = Path.Combine(desktopPath, "HistorialCompras.json");

        // Colección observable para notificar cambios en la UI
        public static ObservableCollection<HistorialCompra> Compras { get; private set; } = new();

        /// <summary>
        /// Carga el historial de compras del usuario indicado.
        /// </summary>
        public static void CargarHistorial(string usuario)
        {
            try
            {
                if (File.Exists(historialPath))
                {
                    string json = File.ReadAllText(historialPath);
                    var historial = JsonSerializer.Deserialize<ObservableCollection<HistorialCompra>>(json) ?? new();

                    // Filtramos solo las compras del usuario actual
                    Compras = new ObservableCollection<HistorialCompra>(
                        historial.Where(h => h.Usuario == usuario)
                    );
                }
                else
                {
                    Compras.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar historial: {ex.Message}");
                Compras.Clear();
            }
        }

        /// <summary>
        /// Agrega una nueva compra al historial y guarda en el archivo JSON.
        /// </summary>
        public static void AgregarCompra(HistorialCompra nuevaCompra)
        {
            try
            {
                // Cargar historial existente
                ObservableCollection<HistorialCompra> historial;
                if (File.Exists(historialPath))
                {
                    string json = File.ReadAllText(historialPath);
                    historial = JsonSerializer.Deserialize<ObservableCollection<HistorialCompra>>(json) ?? new();
                }
                else
                {
                    historial = new ObservableCollection<HistorialCompra>();
                }

                historial.Add(nuevaCompra);

                // Guardar en archivo
                string nuevoJson = JsonSerializer.Serialize(historial, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(historialPath, nuevoJson);

                // Actualizar colección en memoria
                Compras.Add(nuevaCompra);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar historial: {ex.Message}");
            }
        }
    }
}
