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
                MessageBox.Show("Por favor, llena todos los campos para continuar.",
                                "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (contrasenia.Length < 8)
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres por seguridad.",
                                "Contraseña débil", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Por favor, ingresa un número de teléfono válido (entre 10 y 15 dígitos numéricos).",
                                "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Registrando cuenta...", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);

            // Conversión segura de la Fecha de Nacimiento
            DateTime fechaNacimientoParsed;
            if (!DateTime.TryParse(fechaNacimiento, out fechaNacimientoParsed))
            {
                MessageBox.Show("Por favor, selecciona una fecha de nacimiento válida.",
                                "Fecha inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        MessageBox.Show("¡Cuenta registrada con éxito!", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.Navigate(new wLogin());
                        break;
                    case 1:
                        MessageBox.Show("Los datos enviados no cumplen con los requisitos del servidor.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show("El nombre de usuario ya está en uso. Por favor, elige otro.", "Usuario Existente", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 3:
                        MessageBox.Show("El correo electrónico ya está registrado. Si ya tienes cuenta, intenta iniciar sesión.", "Correo Existente", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 4:
                        MessageBox.Show("Ocurrió un error interno en el servidor de base de datos. Intenta más tarde.", "Error del Servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        MessageBox.Show("El servidor devolvió una respuesta desconocida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }

                clienteWCF.Close();
            }
            catch (Exception ex)
            {
                clienteWCF.Abort();
                MessageBox.Show($"Error al conectar con el servidor: {ex.Message}", "Fallo de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
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
