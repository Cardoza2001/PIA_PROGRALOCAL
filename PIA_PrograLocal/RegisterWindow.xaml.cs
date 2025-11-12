using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PIA_PrograLocal.Models;
using System;

namespace PIA_PrograLocal
{
    public sealed partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            this.InitializeComponent();
            RegisterButton.Click += RegisterButton_Click;
            BackToLoginButton.Click += BackToLoginButton_Click;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await ShowDialog("Campos incompletos", "Por favor, llena todos los campos.");
                return;
            }

            if (password != confirmPassword)
            {
                await ShowDialog("Contraseñas no coinciden", "Verifica que ambas contraseñas sean iguales.");
                return;
            }

            try
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = username,
                    Correo = email,
                    Contrasena = password
                };

                UsuarioManager.RegistrarUsuario(nuevoUsuario);
                await ShowDialog("Registro exitoso", $"El usuario '{username}' ha sido registrado correctamente.");

                this.AppWindow.Hide();
                var loginWindow = new MainWindow();
                loginWindow.Activate();
            }
            catch (InvalidOperationException)
            {
                await ShowDialog("Error", "El usuario ya existe.");
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"No se pudo registrar el usuario: {ex.Message}");
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppWindow.Hide();
            var loginWindow = new MainWindow();
            loginWindow.Activate();
        }

        private async System.Threading.Tasks.Task ShowDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = title,
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
