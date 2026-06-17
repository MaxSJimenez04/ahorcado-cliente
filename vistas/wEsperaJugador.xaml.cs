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
    // Ya NO implementa el callback ni crea su propio cliente.
    // Se suscribe al evento JugadorUnido de la conexión compartida (que abrió
    // wCrearPartida y donde el servidor ya registró el callback de este jugador).
    public partial class wEsperaJugador : Page
    {
        private int _idPartidaActual;

        public wEsperaJugador(int idPartida)
        {
            InitializeComponent();
            _idPartidaActual = idPartida;

            // Escuchar cuando el Jugador B se una a la partida
            utils.ConexionPartida.Instancia.JugadorUnido += OnJugadorUnido;
        }

        private void OnJugadorUnido(PartidaServiceRef.PartidaDTO partida)
        {
            // El callback llega en un hilo de fondo: saltar al hilo de UI.
            Dispatcher.Invoke(() =>
            {
                // Dejar de escuchar este evento antes de cambiar de pantalla,
                // pero SIN cerrar la conexión (debe seguir viva en la partida).
                utils.ConexionPartida.Instancia.JugadorUnido -= OnJugadorUnido;

                // Pasamos el DTO completo y 'true' indicando que somos el Juez (Jugador A).
                NavigationService.Navigate(new wPartidaJugador(partida, true));
            });
        }

        private void btnCancelarPartida_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(Properties.Resources.msgConfirmarCancelarPartida,
                                                      Properties.Resources.titCancelarPartida,
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int idJugadorActual = utils.Sesion.Instancia.IdJugador;

                    // Dejar de escuchar y avisar al servidor que se cancela la partida.
                    utils.ConexionPartida.Instancia.JugadorUnido -= OnJugadorUnido;
                    utils.ConexionPartida.Instancia.Cliente.AbandonarPartida(_idPartidaActual, idJugadorActual);
                }
                catch (Exception)
                {
                    // Si algo falla, igual cerramos abajo.
                }
                finally
                {
                    utils.ConexionPartida.Instancia.Cerrar();
                }

                NavigationService.Navigate(new wMenuPrincipal());
            }
        }
    }
}