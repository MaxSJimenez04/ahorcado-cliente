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
    public partial class wEsperaJugador : Page, PartidaServiceRef.IPartidaServiceCallback
    {
        private int _idPartidaActual;
        private PartidaServiceRef.PartidaServiceClient _partidaCliente;

        public wEsperaJugador(int idPartida)
        {
            InitializeComponent();
            _idPartidaActual = idPartida;
            ConectarCanalDuplex();
        }

        private void ConectarCanalDuplex()
        {
            try
            {
                var contexto = new InstanceContext(this);

                _partidaCliente = new PartidaServiceRef.PartidaServiceClient(contexto);

                // Nota: Tu servidor ya asoció este Callback al Jugador en el método CrearPartida, 
                // así que con solo mantener este _partidaCliente vivo, el canal queda abierto y escuchando.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el canal de espera: {ex.Message}", "Fallo de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        private void btnCancelarPartida_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Deseas cancelar la creación de la partida?",
                                                      "Cancelar Partida", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int idJugadorActual = utils.Sesion.Instancia.IdJugador;

                    // Llamamos al servidor para que destruya la partida en la base de datos
                    _partidaCliente.AbandonarPartida(_idPartidaActual, idJugadorActual);
                    _partidaCliente.Close();
                }
                catch (Exception)
                {
                    _partidaCliente?.Abort();
                }

                NavigationService.Navigate(new wMenuPrincipal());
            }
        }



        public void NotificarJugadorUnido(string usuarioJugadorB)
        {
            // WPF requiere que los cambios de interfaz dictados por un hilo externo se hagan usando el Dispatcher
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"¡El jugador {usuarioJugadorB} se ha unido a la partida!\nEl juego va a comenzar.",
                                "Contrincante encontrado", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new wPartidaJugador(true));
            });
        }


        public void NotificarLetraPropuesta(char letra, bool esCorrecta, char[] progresoPalabra, int intentosFallidos)
        {
        }

        public void NotificarFinPartida(int estadoFinal)
        {
        }
    }
}
