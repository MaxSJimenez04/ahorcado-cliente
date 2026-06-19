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
    public partial class wEsperaJugador : Page
    {
        private int _idPartidaActual;

        public wEsperaJugador(int idPartida)
        {
            InitializeComponent();
            _idPartidaActual = idPartida;

            utils.ConexionPartida.Instancia.JugadorUnido += OnJugadorUnido;
        }

        private void OnJugadorUnido(PartidaServiceRef.PartidaDTO partida)
        {
            Dispatcher.Invoke(() =>
            {
                utils.ConexionPartida.Instancia.JugadorUnido -= OnJugadorUnido;

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

                    utils.ConexionPartida.Instancia.JugadorUnido -= OnJugadorUnido;
                    utils.ConexionPartida.Instancia.Cliente.AbandonarPartida(_idPartidaActual, idJugadorActual);
                }
                catch (Exception)
                {
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