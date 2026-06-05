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

namespace ClienteAhorcado.vistas
{
    /// <summary>
    /// Lógica de interacción para wRegistro.xaml
    /// </summary>
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
            // El botón cancelar hace exactamente lo mismo que regresar en este flujo
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
            // Validaciones y captura de datos para registrar al usuario
            string nombreUsuario = txtNombreUsuario.Text;
            string correo = txtCorreo.Text;
            string contrasenia = _contraseniaVisible ? txtContraseniaVisible.Text : pbContraseniaOculta.Password;
            string fechaNacimiento = txtFechaNacimiento.Text;
            string telefono = txtTelefono.Text;

            // TODO: Agregar lógica de creación de cuenta
            MessageBox.Show("Registrando cuenta...", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService.Navigate(new wMenuPrincipal());
        }
    }
}
