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
using System.ServiceModel;

namespace ClienteAhorcado.vistas
{
    public partial class wUnirsePartida : Page, PartidaServiceRef.IPartidaServiceCallback
    {
        public wUnirsePartida()
        {
            InitializeComponent();
            CargarPartidas();
        }

        private void CargarPartidas()
        {
            // Toda llamada a PartidaService requiere InstanceContext
            var contexto = new InstanceContext(this);
            var partidaSrv = new PartidaServiceRef.PartidaServiceClient(contexto);

            try
            {
                var listaPartidas = partidaSrv.ObtenerPartidasDisponibles();

                // Le pasamos la lista de DTOs directamente al ListBox. 
                // La DataTemplate en XAML se encarga del resto.
                lbPartidas.ItemsSource = listaPartidas;

                partidaSrv.Close();
            }
            catch (Exception ex)
            {
                partidaSrv.Abort();
                MessageBox.Show($"Error al cargar la lista de partidas: {ex.Message}", "Fallo de comunicación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarPartidas();
        }

        private void btnUnirsePartida_Click(object sender, RoutedEventArgs e)
        {
            if (lbPartidas.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona una partida de la lista para unirte.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Al usar Data Binding, el SelectedItem ES nuestro PartidaLobbyDTO
            var partidaSeleccionada = (PartidaServiceRef.PartidaLobbyDTO)lbPartidas.SelectedItem;

            int idJugadorActual = utils.Sesion.Instancia.IdJugador;

            var contexto = new InstanceContext(this);
            var partidaSrv = new PartidaServiceRef.PartidaServiceClient(contexto);

            try
            {
                // Intentamos unirnos a la partida seleccionada
                var estadoPartida = partidaSrv.UnirseAPartida(partidaSeleccionada.idPartida, idJugadorActual);

                if (estadoPartida != null)
                {
                    // Si nos devuelve el objeto, significa que el servidor nos aceptó en la sala
                    MessageBox.Show($"Conectado a la partida '{partidaSeleccionada.nombrePartida}'.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new wPartidaJugador(false));
                }
                else
                {
                    // Alguien más se unió un milisegundo antes o el creador canceló
                    MessageBox.Show("No se pudo unir a la partida. Es posible que ya no esté disponible.", "Partida no disponible", MessageBoxButton.OK, MessageBoxImage.Stop);
                    CargarPartidas();
                }

                partidaSrv.Close();
            }
            catch (Exception ex)
            {
                partidaSrv.Abort();
                MessageBox.Show($"Error al intentar unirse a la partida: {ex.Message}", "Fallo de comunicación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void NotificarJugadorUnido(string usuarioJugadorB)
        {
        }

        public void NotificarLetraPropuesta(char letra, bool esCorrecta, char[] progresoPalabra, int intentosFallidos)
        {
        }

        public void NotificarFinPartida(int estadoFinal)
        {
        }
    }
}
