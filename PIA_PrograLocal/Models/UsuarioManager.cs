using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PIA_PrograLocal.Models
{
    public static class UsuarioManager
    {
        private static readonly string desktopPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "usuarios.json"
        );

        public static List<Usuario> Usuarios { get; private set; } = new();

        static UsuarioManager()
        {
            CargarUsuarios();
        }

        public static void CargarUsuarios()
        {
            try
            {
                if (File.Exists(desktopPath))
                {
                    string json = File.ReadAllText(desktopPath);
                    var usuariosCargados = JsonSerializer.Deserialize<List<Usuario>>(json);
                    Usuarios = usuariosCargados ?? new List<Usuario>();
                }
                else
                {
                    Usuarios = new List<Usuario>();
                    GuardarUsuarios();
                }
            }
            catch
            {
                Usuarios = new List<Usuario>();
            }
        }

        public static void GuardarUsuarios()
        {
            try
            {
                string json = JsonSerializer.Serialize(Usuarios, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(desktopPath, json);
            }
            catch { }
        }

        public static void RegistrarUsuario(Usuario nuevoUsuario)
        {
            if (nuevoUsuario == null)
                throw new ArgumentNullException(nameof(nuevoUsuario));

            if (Usuarios.Any(u =>
                u.Nombre != null &&
                u.Nombre.Equals(nuevoUsuario.Nombre, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("El usuario ya existe.");

            Usuarios.Add(nuevoUsuario);
            GuardarUsuarios();
        }

        public static Usuario? ValidarUsuario(string nombre, string contrasena)
        {
            return Usuarios.FirstOrDefault(u =>
                u.Nombre != null &&
                u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                u.Contrasena == contrasena);
        }
    }
}
