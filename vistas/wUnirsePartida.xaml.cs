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

                bool esEspanol = utils.Sesion.Instancia.IdIdioma == 1;

                // CAMBIO AQUÍ: Ahora instanciamos PartidaLobbyUI explícitamente
                var listaUI = listaPartidas.Select(p => new PartidaLobbyUI
                {
                    idPartida = p.idPartida,
                    nombrePartida = p.nombrePartida,
                    usuarioJugadorA = p.usuarioJugadorA,
                    correoJugadorA = p.correoJugadorA,
                    fechaCreacion = esEspanol ? p.fechaCreacion.ToString("dd/MM/yyyy")
                                              : p.fechaCreacion.ToString("MM/dd/yyyy")
                }).ToList();

                lbPartidas.ItemsSource = listaUI;
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

            var partidaSrv = utils.ConexionPartida.Instancia.Conectar();

            try
            {
                // CAMBIO AQUÍ: Hacemos el cast a la nueva clase y de forma segura dentro del try
                var partidaSeleccionada = (PartidaLobbyUI)lbPartidas.SelectedItem;
                int idJugadorActual = utils.Sesion.Instancia.IdJugador;

                var estadoPartida = partidaSrv.UnirseAPartida(partidaSeleccionada.idPartida, idJugadorActual);

                if (estadoPartida != null)
                {
                    NavigationService.Navigate(new wPartidaJugador(estadoPartida, false));
                }
                else
                {
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

        public class PartidaLobbyUI
        {
            public int idPartida { get; set; }
            public string nombrePartida { get; set; }
            public string usuarioJugadorA { get; set; }
            public string correoJugadorA { get; set; }
            public string fechaCreacion { get; set; }
        }
    }
}