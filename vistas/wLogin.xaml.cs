using ClienteAhorcado.SesionServiceRef;
using ClienteAhorcado.UsuarioServiceRef;
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
    public partial class wLogin : Page
    {
        private bool _contraseniaVisible = false;

        public wLogin()
        {
            InitializeComponent();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wInicio());
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

        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtNombreUsuario.Text;
            string contrasenia = _contraseniaVisible ? txtContraseniaVisible.Text : pbContraseniaOculta.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasenia))
            {
                MessageBox.Show("Por favor, ingresa tu usuario y contraseña.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cliente NUEVO en cada intento, sin using
            var sesion = new SesionServiceClient();

            try
            {
                var resultado = sesion.IniciaSesion(usuario, contrasenia);
                sesion.Close(); // Cierre manual solo si todo salió bien

                switch (resultado.Key)
                {
                    case 1:
                        MessageBox.Show("Por favor, ingresa usuario y contraseña.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show("No se encontró el usuario", "Usuario no encontrado", MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    case 3:
                        MessageBox.Show("Ya hay una sesión activa", "Sesión Activa", MessageBoxButton.OK, MessageBoxImage.Hand);
                        break;
                    case 4:
                        MessageBox.Show("No hay conexión con el servidor, intente de nuevo más tarde", "Sin conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                        NavigationService.Navigate(new wInicio());
                        break;
                    case 0:
                        NavigationService.Navigate(new wMenuPrincipal());
                        break;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                sesion.Abort(); // Abort nunca lanza excepción, limpia el canal a la fuerza
                MessageBox.Show("No se pudo conectar con el servidor, intente más tarde.",
                                "Sin conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
            catch (System.ServiceModel.CommunicationException)
            {
                sesion.Abort();
                MessageBox.Show("Error de comunicación con el servidor, intente más tarde.",
                                "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
            catch (TimeoutException)
            {
                sesion.Abort();
                MessageBox.Show("El servidor tardó demasiado en responder.",
                                "Tiempo agotado", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
        }

        private void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wRegistro());
        }
    }
}
