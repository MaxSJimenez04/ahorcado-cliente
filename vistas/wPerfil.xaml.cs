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
    public partial class wPerfil : Page
    {
        public wPerfil()
        {
            InitializeComponent();
            CargarDatosPerfil();
        }

        private void CargarDatosPerfil()
        {
            string usuarioLogueado = utils.Sesion.Instancia.Usuario;

            if (string.IsNullOrEmpty(usuarioLogueado))
            {
                MessageBox.Show("No se encontró una sesión activa. Volviendo al inicio.", "Error de Sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wInicio());
                return;
            }

            // Instanciamos el cliente WCF
            var usuarioSrv = new UsuarioServiceRef.UsuarioServiceClient();

            try
            {
                // Solicitamos los datos al servidor
                var datosPerfil = usuarioSrv.ObtenerDatosJugador(usuarioLogueado);

                if (datosPerfil != null)
                {
                    // ACTUALIZAR SINGLETON
                    var sesionLocal = utils.Sesion.Instancia;

                    sesionLocal.IdJugador = datosPerfil.idJugador;
                    sesionLocal.Nombre = datosPerfil.nombre;
                    sesionLocal.PrimerApellido = datosPerfil.primerApellido;
                    sesionLocal.SegundoApellido = datosPerfil.segundoApellido;
                    sesionLocal.Correo = datosPerfil.correo;
                    sesionLocal.Telefono = datosPerfil.telefono;
                    sesionLocal.FechaNacimiento = datosPerfil.fechaNacimiento;
                    sesionLocal.Puntos = datosPerfil.puntos;

                    // POBLAR LA INTERFAZ GRÁFICA
                    txtNombre.Text = datosPerfil.nombre;
                    txtPrimerApellido.Text = datosPerfil.primerApellido;
                    txtSegundoApellido.Text = datosPerfil.segundoApellido;
                    txtNombreUsuario.Text = datosPerfil.usuario;
                    txtCorreo.Text = datosPerfil.correo;
                    txtTelefono.Text = datosPerfil.telefono;

                    txtFechaNacimiento.Text = datosPerfil.fechaNacimiento.ToString("dd/MM/yyyy");
                }
                else
                {
                    MessageBox.Show("No se pudieron recuperar los datos del perfil desde el servidor.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                usuarioSrv.Close();
            }
            catch (Exception ex)
            {
                usuarioSrv.Abort();
                MessageBox.Show($"Error al conectar con el servidor: {ex.Message}\nSe usarán los datos locales de sesión.",
                                "Fallo de comunicación", MessageBoxButton.OK, MessageBoxImage.Error);

                // Respaldo (Fallback): Si falla el servidor, cargamos lo que tenga el Singleton localmente
                CargarDatosLocales();
            }
        }

        private void CargarDatosLocales()
        {
            var sesionLocal = utils.Sesion.Instancia;

            if (!string.IsNullOrEmpty(sesionLocal.Usuario))
            {
                txtNombre.Text = sesionLocal.Nombre;
                txtPrimerApellido.Text = sesionLocal.PrimerApellido;
                txtSegundoApellido.Text = sesionLocal.SegundoApellido;
                txtNombreUsuario.Text = sesionLocal.Usuario;
                txtCorreo.Text = sesionLocal.Correo;
                txtTelefono.Text = sesionLocal.Telefono;

                // Verificar fecha válida antes de formatear
                if (sesionLocal.FechaNacimiento != DateTime.MinValue)
                {
                    txtFechaNacimiento.Text = sesionLocal.FechaNacimiento.ToString("dd/MM/yyyy");
                }
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wMenuPrincipal());
        }

        private void btnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wEstadisticas());
        }

        private void btnEditarPerfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wEditarPerfil());
        }
    }
}