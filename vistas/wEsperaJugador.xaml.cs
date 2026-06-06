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
        public wEsperaJugador()
        {
            InitializeComponent();
        }

        private void btnAbandonarPartida_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Deseas cancelar la creación de la partida?",
                                                      "Cancelar Partida", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        private void btnIniciarPartidaDemo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wPartidaJugador(true));
        }
    }
}
