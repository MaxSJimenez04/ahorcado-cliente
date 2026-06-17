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
using ClienteAhorcado.PartidaServiceRef;

namespace ClienteAhorcado.vistas
{
    // Ya NO implementa el callback ni crea su propio cliente.
    // Usa la conexión compartida: para listar y, sobre todo, para unirse
    // (así el servidor registra el callback del Jugador B en la conexión que
    // seguirá viva dentro de wPartidaJugador).
    public partial class wUnirsePartida : Page
    {
        public wUnirsePartida()
        {
            InitializeComponent();
            CargarPartidas();
        }

        private void CargarPartidas()
        {
            var partidaSrv = utils.ConexionPartida.Instancia.Conectar();
            try
            {
                var listaPartidas = partidaSrv.ObtenerPartidasDisponibles();
                lbPartidas.ItemsSource = listaPartidas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.msgErrorCargarPartidas, ex.Message),
                                Properties.Resources.titFalloComunicacion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Salimos sin unirnos: cerramos la conexión que abrimos para listar.
            utils.ConexionPartida.Instancia.Cerrar();

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
                MessageBox.Show(Properties.Resources.msgSeleccionarPartida, Properties.Resources.titAviso, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var partidaSeleccionada = (PartidaServiceRef.PartidaLobbyDTO)lbPartidas.SelectedItem;
            int idJugadorActual = utils.Sesion.Instancia.IdJugador;

            var partidaSrv = utils.ConexionPartida.Instancia.Conectar();

            try
            {
                var estadoPartida = partidaSrv.UnirseAPartida(partidaSeleccionada.idPartida, idJugadorActual);

                if (estadoPartida != null)
                {
                    // Nos aceptaron: NO cerramos la conexión, debe seguir viva en la partida.
                    NavigationService.Navigate(new wPartidaJugador(estadoPartida, false));
                }
                else
                {
                    // Alguien más se unió un instante antes o el creador canceló.
                    MessageBox.Show(Properties.Resources.msgPartidaNoDisponible, Properties.Resources.titPartidaNoDisponible, MessageBoxButton.OK, MessageBoxImage.Stop);
                    CargarPartidas();
                }
            }
            catch (Exception ex)
            {
                utils.ConexionPartida.Instancia.Cerrar();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorUnirsePartida, ex.Message), Properties.Resources.titFalloComunicacion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}