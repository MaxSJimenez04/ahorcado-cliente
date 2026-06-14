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
    public partial class wEditarPerfil : Page
    {
        private bool _contraseniaVisible = false;

        public wEditarPerfil()
        {
            InitializeComponent();
            CargarDatosDesdeSesion();
        }

        private void CargarDatosDesdeSesion()
        {
            var sesion = utils.Sesion.Instancia;

            // Prellenamos los campos con la información actual
            txtNombre.Text = sesion.Nombre;
            txtPrimerApellido.Text = sesion.PrimerApellido;
            txtSegundoApellido.Text = sesion.SegundoApellido;
            txtNombreUsuario.Text = sesion.Usuario;
            txtCorreo.Text = sesion.Correo;
            txtTelefono.Text = sesion.Telefono;

            if (sesion.FechaNacimiento != DateTime.MinValue)
            {
                dpFechaNacimiento.SelectedDate = sesion.FechaNacimiento;
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            RegresarAPerfil();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            RegresarAPerfil();
        }

        private void RegresarAPerfil()
        {
            NavigationService.Navigate(new wPerfil());
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

        private void btnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text;
            string primerApellido = txtPrimerApellido.Text;
            string segundoApellido = txtSegundoApellido.Text;
            string telefono = txtTelefono.Text;
            string contrasenia = _contraseniaVisible ? txtContraseniaVisible.Text : pbContraseniaOculta.Password;
            DateTime? fechaNacimiento = dpFechaNacimiento.SelectedDate;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(primerApellido) ||
                string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(contrasenia) ||
                fechaNacimiento == null)
            {
                MessageBox.Show(Properties.Resources.MsgCamposVacios, Properties.Resources.TitDatosIncompletos, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (contrasenia.Length < 8)
            {
                MessageBox.Show(Properties.Resources.MsgContraseniaCorta, Properties.Resources.titContraseniaDebil, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsTelefonoValido(telefono))
            {
                MessageBox.Show(Properties.Resources.MsgTelefonoInvalido, Properties.Resources.titTelefonoInvalido, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sesion = utils.Sesion.Instancia;
            var jugadorActualizado = new UsuarioServiceRef.JugadorDTO
            {
                idJugador = sesion.IdJugador,
                usuario = sesion.Usuario,
                correo = sesion.Correo,
                nombre = nombre,
                primerApellido = primerApellido,
                segundoApellido = segundoApellido,
                telefono = telefono,
                fechaNacimiento = fechaNacimiento.Value,
                contrasena = contrasenia
            };

            // Llamada al servicio
            var usuarioSrv = new UsuarioServiceRef.UsuarioServiceClient();
            try
            {
                int estadoActualizacion = usuarioSrv.ActualizarJugador(jugadorActualizado);

                switch (estadoActualizacion)
                {
                    case 0:
                        // Si es exitoso, actualizamos nuestro Singleton
                        sesion.Nombre = nombre;
                        sesion.PrimerApellido = primerApellido;
                        sesion.SegundoApellido = segundoApellido;
                        sesion.Telefono = telefono;
                        sesion.FechaNacimiento = fechaNacimiento.Value;

                        MessageBox.Show(Properties.Resources.MsgCambiosGuardados, Properties.Resources.titExito, MessageBoxButton.OK, MessageBoxImage.Information);
                        RegresarAPerfil();
                        break;
                    case 1:
                        MessageBox.Show(Properties.Resources.MsgDatosInvalidos, Properties.Resources.titErrorValidacion, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show(Properties.Resources.MsgUsuarioNoEncontrado, Properties.Resources.titError, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case 3:
                        MessageBox.Show(Properties.Resources.MsgUsuarioOcupado, Properties.Resources.titConflicto, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 4:
                        MessageBox.Show(Properties.Resources.MsgErrorServidor, Properties.Resources.titError, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
                usuarioSrv.Close();
            }
            catch (Exception ex)
            {
                usuarioSrv.Abort();
                MessageBox.Show(string.Format(Properties.Resources.MsgErrorConexion, ex.Message), Properties.Resources.titFalloConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool EsTelefonoValido(string telefono)
        {
            string patron = @"^\d{10,15}$";
            return Regex.IsMatch(telefono, patron);
        }
    }
}
