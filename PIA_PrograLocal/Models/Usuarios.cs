using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIA_PrograLocal.Models
{
    public class Usuario
    {
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }

        public Usuario() { }

        public Usuario(string nombreUsuario, string correo, string contrasena)
        {
            Nombre = nombreUsuario;
            Correo = correo;
            Contrasena = contrasena;
        }
    }
}