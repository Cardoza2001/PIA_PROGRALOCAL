using System;
using System.Collections.Generic;
using System.Linq;

namespace PIA_PrograLocal.Models
{
    public class HistorialCompra
    {
        public string Usuario { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
        public List<Auto> Autos { get; set; } = new();
        public string RutaPDF { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public double Total { get; set; }

        public string FechaCompra => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string DescripcionCompra => string.Join(", ", Autos.Select(a => $"{a.Marca} {a.Modelo}"));
        public string TotalCompra => $"Total: {Total:C}";
    }
}
