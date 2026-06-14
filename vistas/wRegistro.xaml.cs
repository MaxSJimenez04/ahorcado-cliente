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
            string nombre = txtNombre.Text;
            string primerApellido = txtPrimerApellido.Text;
            string segundoApellido = txtSegundoApellido.Text;
            string nombreUsuario = txtNombreUsuario.Text;
            string correo = txtCorreo.Text;
            string telefono = txtTelefono.Text;
            string fechaNacimiento = dpFechaNacimiento.Text;
            string contrasenia = _contraseniaVisible ? txtContraseniaVisible.Text : pbContraseniaOculta.Password;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(primerApellido) ||
                string.IsNullOrWhiteSpace(segundoApellido) || string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasenia) ||
                string.IsNullOrWhiteSpace(fechaNacimiento) || string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show(Properties.Resources.msgLlenarCampos, Properties.Resources.TitDatosIncompletos, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (contrasenia.Length < 8)
            {
                MessageBox.Show(Properties.Resources.msgContraseniaSeguridad, Properties.Resources.titContraseniaDebil, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show(Properties.Resources.msgCorreoInvalido, Properties.Resources.titCorreoInvalido, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsTelefonoValido(telefono))
            {
                MessageBox.Show(Properties.Resources.MsgTelefonoInvalido, Properties.Resources.titTelefonoInvalido, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(Properties.Resources.msgRegistrandoCuenta, Properties.Resources.titRegistro, MessageBoxButton.OK, MessageBoxImage.Information);

            // Conversión segura de la Fecha de Nacimiento
            DateTime fechaNacimientoParsed;
            if (!DateTime.TryParse(fechaNacimiento, out fechaNacimientoParsed))
            {
                MessageBox.Show(Properties.Resources.msgFechaInvalida, Properties.Resources.titFechaInvalida, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevoJugador = new UsuarioServiceRef.JugadorDTO
            {
                nombre = nombre,
                primerApellido = primerApellido,
                segundoApellido = segundoApellido,
                usuario = nombreUsuario,
                correo = correo,
                telefono = telefono,
                fechaNacimiento = fechaNacimientoParsed,
                contrasena = contrasenia
            };

            // Llamada al servicio WCF y manejo de estados
            var clienteWCF = new UsuarioServiceRef.UsuarioServiceClient();
            try
            {
                // Llamamos al método y guardamos el código de estado (0, 1, 2, 3, 4)
                int estadoRegistro = clienteWCF.RegistrarJugador(nuevoJugador);

                switch (estadoRegistro)
                {
                    case 0:
                        MessageBox.Show(Properties.Resources.msgRegistroExitoso, Properties.Resources.titRegistroExitoso, MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.Navigate(new wLogin());
                        break;
                    case 1:
                        MessageBox.Show(Properties.Resources.msgDatosInvalidosServidor, Properties.Resources.titErrorValidacion, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show(Properties.Resources.msgUsuarioEnUso, Properties.Resources.titUsuarioExistente, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 3:
                        MessageBox.Show(Properties.Resources.msgCorreoRegistrado, Properties.Resources.titCorreoExistente, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 4:
                        MessageBox.Show(Properties.Resources.msgErrorBD, Properties.Resources.titErrorServidor, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        MessageBox.Show(Properties.Resources.msgRespuestaDesconocida, Properties.Resources.titError, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }

                clienteWCF.Close();
            }
            catch (Exception ex)
            {
                clienteWCF.Abort();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorConectarServidor, ex.Message), Properties.Resources.titFalloConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool EsCorreoValido(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, patron);
        }

        private bool EsTelefonoValido(string telefono)
        {
            string patron = @"^\d{10,15}$";
            return Regex.IsMatch(telefono, patron);
        }
    }
}
