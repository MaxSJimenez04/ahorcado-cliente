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
                MessageBox.Show(string.Format(Properties.Resources.msgErrorCanalEspera, ex.Message),
                                Properties.Resources.titFalloConexion,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                NavigationService.Navigate(new wMenuPrincipal());
            }
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



        public void NotificarJugadorUnido(PartidaServiceRef.PartidaDTO partida)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(string.Format(Properties.Resources.msgJugadorUnido, partida.usuarioJugadorB),
                                Properties.Resources.titContrincanteEncontrado,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // Pasamos el DTO al constructor y 'true' indicando que somos el Juez
                NavigationService.Navigate(new wPartidaJugador(partida, true));
            });
        }


        public void NotificarLetraPropuesta(char letra, bool esCorrecta, char[] progresoPalabra, int intentosFallidos)
        {
        }

        public void NotificarFinPartida(int estadoFinal)
        {
        }

        public void NotificarLetraParaJuzgar(char letra)
        {
        }

        public void NotificarErrorJuicio(char letra, bool eraCorrecta)
        {
        }
    }
}
