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
                MessageBox.Show(Properties.Resources.msgIngresarCredenciales, Properties.Resources.titCamposVacios, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sesion = new SesionServiceClient();

            try
            {
                var resultado = sesion.IniciaSesion(usuario, contrasenia);
                sesion.Close();

                switch (resultado.Key)
                {
                    case 1:
                        MessageBox.Show(Properties.Resources.msgIngresarCredenciales, Properties.Resources.titCamposVacios, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show(Properties.Resources.msgCredencialesIncorrectas, Properties.Resources.titCredencialesInvalidas, MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    case 3:
                        MessageBox.Show(Properties.Resources.msgSesionActiva, Properties.Resources.titSesionActiva, MessageBoxButton.OK, MessageBoxImage.Hand);
                        break;
                    case 4:
                        MessageBox.Show(Properties.Resources.msgErrorBD, Properties.Resources.titErrorServidor, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case 5:
                        MessageBox.Show(Properties.Resources.msgErrorInesperadoServidor, Properties.Resources.titError, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case 0:
                        var sesionGlobal = utils.Sesion.Instancia;
                        var datosRecibidos = resultado.Value;

                        sesionGlobal.IdJugador = datosRecibidos.idJugador;
                        sesionGlobal.Usuario = datosRecibidos.usuario;
                        sesionGlobal.Nombre = datosRecibidos.nombre;
                        sesionGlobal.PrimerApellido = datosRecibidos.primerApellido;
                        sesionGlobal.SegundoApellido = datosRecibidos.segundoApellido;
                        sesionGlobal.Correo = datosRecibidos.correo;
                        sesionGlobal.Telefono = datosRecibidos.telefono;
                        sesionGlobal.FechaNacimiento = datosRecibidos.fechaNacimiento;

                        NavigationService.Navigate(new wMenuPrincipal());
                        break;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                sesion.Abort();
                MessageBox.Show(Properties.Resources.msgSinConexion, Properties.Resources.titSinConexion, MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
            catch (System.ServiceModel.CommunicationException)
            {
                sesion.Abort();
                MessageBox.Show(Properties.Resources.msgErrorComunicacion, Properties.Resources.titErrorConexion, MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
            catch (TimeoutException)
            {
                sesion.Abort();
                MessageBox.Show(Properties.Resources.msgTiempoAgotado, Properties.Resources.titTiempoAgotado, MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
            }
        }

        private void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wRegistro());
        }
    }
}