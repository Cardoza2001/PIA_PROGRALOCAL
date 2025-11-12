using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PIA_PrograLocal.Models
{
    public class Auto
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public double Precio { get; set; }
        public string PrecioDisplay => $"Precio: {Precio:C}";
    }

    public static class AutosData
    {
        private static readonly string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static readonly string filePath = Path.Combine(desktopPath, "autos.json");

        public static List<Auto> Autos { get; private set; } = new();

        public static void Inicializar()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Autos = JsonSerializer.Deserialize<List<Auto>>(json) ?? new List<Auto>();
                }
                else
                {
                    CargarAutosIniciales();
                    GuardarAutos();
                }
            }
            catch
            {
                CargarAutosIniciales();
            }
        }

        public static void GuardarAutos()
        {
            try
            {
                string json = JsonSerializer.Serialize(Autos, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch { }
        }

        private static void CargarAutosIniciales()
        {
            Autos = new List<Auto>
            {
                new Auto { Marca = "Toyota", Modelo = "Corolla", Descripcion = "Sedán, 5p, 1.8L, CVT, 4 cil, gas.", Precio = 345000 },
                new Auto { Marca = "Toyota", Modelo = "Yaris HB", Descripcion = "Hatchback, 5p, 1.5L, man, gas, 5 pers.", Precio = 299000 },
                new Auto { Marca = "Toyota", Modelo = "Camry", Descripcion = "Sedán, 2.5L, aut, 5p, gas, 4 cil.", Precio = 565000 },
                new Auto { Marca = "Toyota", Modelo = "RAV4", Descripcion = "SUV, 5p, 2.5L, aut, AWD, gas.", Precio = 685000 },
                new Auto { Marca = "Toyota", Modelo = "Hilux", Descripcion = "Pick-up, 4x4, 2.8L, diésel, doble cab.", Precio = 715000 },
                new Auto { Marca = "Toyota", Modelo = "Tacoma", Descripcion = "Pick-up, 4x4, 3.5L, aut, gas, 5p.", Precio = 830000 },

                new Auto { Marca = "Honda", Modelo = "Civic", Descripcion = "Sedán, 1.5L turbo, CVT, 5p, gas.", Precio = 485000 },
                new Auto { Marca = "Honda", Modelo = "Accord", Descripcion = "Sedán, 2.0L, aut, 4p, gas.", Precio = 610000 },
                new Auto { Marca = "Honda", Modelo = "CR-V", Descripcion = "SUV, 5p, 1.5L turbo, CVT, gas.", Precio = 705000 },
                new Auto { Marca = "Honda", Modelo = "HR-V", Descripcion = "SUV compacta, 1.8L, CVT, gas, 5 pers.", Precio = 545000 },
                new Auto { Marca = "Honda", Modelo = "Pilot", Descripcion = "SUV, 7p, 3.5L V6, AWD, aut.", Precio = 930000 },
                new Auto { Marca = "Honda", Modelo = "Ridgeline", Descripcion = "Pick-up, V6 3.5L, aut, AWD, 5p.", Precio = 880000 },

                new Auto { Marca = "Nissan", Modelo = "Sentra", Descripcion = "Sedán, 1.8L, CVT, gas, 5p.", Precio = 385000 },
                new Auto { Marca = "Nissan", Modelo = "Versa", Descripcion = "Sedán, 1.6L, man, gas, 5p.", Precio = 310000 },
                new Auto { Marca = "Nissan", Modelo = "Altima", Descripcion = "Sedán, 2.5L, aut, AWD, gas.", Precio = 540000 },
                new Auto { Marca = "Nissan", Modelo = "X-Trail", Descripcion = "SUV, 2.5L, aut, AWD, gas.", Precio = 690000 },
                new Auto { Marca = "Nissan", Modelo = "Frontier", Descripcion = "Pick-up, 4x4, 3.8L V6, aut.", Precio = 755000 },
                new Auto { Marca = "Nissan", Modelo = "March", Descripcion = "Hatchback, 1.6L, man, gas, 5p.", Precio = 270000 },

                new Auto { Marca = "Ford", Modelo = "Focus", Descripcion = "Sedán, 2.0L, aut, gas, 5p.", Precio = 420000 },
                new Auto { Marca = "Ford", Modelo = "Fusion", Descripcion = "Sedán, 2.5L, aut, gas, 4p.", Precio = 510000 },
                new Auto { Marca = "Ford", Modelo = "Escape", Descripcion = "SUV, 1.5L turbo, aut, AWD.", Precio = 655000 },
                new Auto { Marca = "Ford", Modelo = "Bronco Sport", Descripcion = "SUV, 2.0L turbo, AWD, gas.", Precio = 835000 },
                new Auto { Marca = "Ford", Modelo = "Ranger", Descripcion = "Pick-up, 4x4, 2.3L turbo, aut.", Precio = 785000 },
                new Auto { Marca = "Ford", Modelo = "Mustang", Descripcion = "Coupé, V8 5.0L, man, gas, 2p.", Precio = 1150000 },

                new Auto { Marca = "Chevrolet", Modelo = "Onix", Descripcion = "Sedán, 1.2L turbo, aut, gas, 5p.", Precio = 345000 },
                new Auto { Marca = "Chevrolet", Modelo = "Aveo", Descripcion = "Sedán, 1.5L, man, gas, 5p.", Precio = 290000 },
                new Auto { Marca = "Chevrolet", Modelo = "Tracker", Descripcion = "SUV, 1.2L turbo, aut, gas.", Precio = 470000 },
                new Auto { Marca = "Chevrolet", Modelo = "Equinox", Descripcion = "SUV, 1.5L turbo, aut, AWD.", Precio = 635000 },
                new Auto { Marca = "Chevrolet", Modelo = "Silverado", Descripcion = "Pick-up, 4x4, V8 5.3L, aut.", Precio = 995000 },
                new Auto { Marca = "Chevrolet", Modelo = "Camaro", Descripcion = "Coupé, V6 3.6L, aut, gas, 2p.", Precio = 1100000 },

                new Auto { Marca = "BMW", Modelo = "Serie 1", Descripcion = "Hatchback, 1.5L turbo, aut.", Precio = 675000 },
                new Auto { Marca = "BMW", Modelo = "Serie 3", Descripcion = "Sedán, 2.0L turbo, aut, gas.", Precio = 985000 },
                new Auto { Marca = "BMW", Modelo = "Serie 5", Descripcion = "Sedán, 3.0L, aut, 5p, gas.", Precio = 1240000 },
                new Auto { Marca = "BMW", Modelo = "X1", Descripcion = "SUV compacta, 1.5L turbo, AWD.", Precio = 890000 },
                new Auto { Marca = "BMW", Modelo = "X3", Descripcion = "SUV mediana, 2.0L turbo, AWD.", Precio = 1090000 },
                new Auto { Marca = "BMW", Modelo = "Z4", Descripcion = "Roadster, 2p, 2.0L turbo, aut.", Precio = 1360000 },
            };
        }
    }
}
