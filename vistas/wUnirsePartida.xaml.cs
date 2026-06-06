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
    public partial class wUnirsePartida : Page
    {
        public wUnirsePartida()
        {
            InitializeComponent();
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
            MessageBox.Show("Actualizando lista de partidas...", "Actualizar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUnirsePartida_Click(object sender, RoutedEventArgs e)
        {
            if (lbPartidas.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona una partida de la lista para unirte.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Conectando a la partida...", "Unirse", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new wPartidaJugador(false));
        }
    }
}
