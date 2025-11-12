using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PIA_PrograLocal.Models;
using PIA_PrograLocal.Views;
using System;
using System.Linq;

namespace PIA_PrograLocal
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            LoginButton.Click += LoginButton_Click;
            RegisterButton.Click += RegisterButton_Click;

            // Cargar los usuarios desde archivo JSON
            UsuarioManager.CargarUsuarios();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Carga los usuarios antes de validar (por seguridad)
            UsuarioManager.CargarUsuarios();

            var usuario = UsuarioManager.ValidarUsuario(username, password);

            if (usuario != null)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Acceso concedido",
                    Content = $"Bienvenido, {usuario.Nombre}.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();

                var tiendaWindow = new TiendaWindow();
                tiendaWindow.Activate();

                this.Close();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Error de inicio de sesión",
                    Content = "Usuario o contraseña incorrectos.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Activate();
            this.Close();
        }
    }
}
