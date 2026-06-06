using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace ClienteAhorcado.vistas
{
    public partial class wRegistro : Page
    {
        private bool _contraseniaVisible = false;

        public wRegistro()
        {
            InitializeComponent();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            VolverAtras();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VolverAtras();
        }

        private void VolverAtras()
        {
            NavigationService.Navigate(new wLogin());
        }

        private void btnMostrarContrasenia_Click(object sender, RoutedEventArgs e)
        {
            _contraseniaVisible = !_contraseniaVisible;

            if (_contraseniaVisible)
            {
                txtContraseniaVisible.Text = pbContraseniaOculta.Password;
                pbContraseniaOculta.Visibility = Visibility.Collapsed;
                txtContraseniaVisible.Visibility = Visibility.Visible;
            }
            else
            {
                pbContraseniaOculta.Password = txtContraseniaVisible.Text;
                txtContraseniaVisible.Visibility = Visibility.Collapsed;
                pbContraseniaOculta.Visibility = Visibility.Visible;
            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtNombreUsuario.Text;
            string correo = txtCorreo.Text;
            string contrasenia = _contraseniaVisible ? txtContraseniaVisible.Text : pbContraseniaOculta.Password;
            string fechaNacimiento = dpFechaNacimiento.Text;
            string telefono = txtTelefono.Text;

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasenia) || string.IsNullOrWhiteSpace(fechaNacimiento) ||
                string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show("Por favor, llena todos los campos para continuar.",
                                "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show("Por favor, ingresa un correo electrónico válido (ejemplo: usuario@correo.com).",
                                "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsTelefonoValido(telefono))
            {
                MessageBox.Show("Por favor, ingresa un número de teléfono válido de 10 dígitos.",
                                "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Registrando cuenta...", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService.Navigate(new wMenuPrincipal());
        }

        private bool EsCorreoValido(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, patron);
        }

        private bool EsTelefonoValido(string telefono)
        {
            string patron = @"^\d{10}$";
            return Regex.IsMatch(telefono, patron);
        }
    }
}
