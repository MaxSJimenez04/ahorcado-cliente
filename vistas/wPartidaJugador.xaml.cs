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
    public partial class wPartidaJugador : Page
    {
        private bool _esJuez;

        public wPartidaJugador(bool esJuez = false)
        {
            InitializeComponent();
            _esJuez = esJuez;
            ConfigurarInterfazPorRol();
        }

        private void ConfigurarInterfazPorRol()
        {
            if (_esJuez)
            {
                panelAdivinador.Visibility = Visibility.Collapsed;
                panelJuez.Visibility = Visibility.Visible;
            }
            else
            {
                panelAdivinador.Visibility = Visibility.Visible;
                panelJuez.Visibility = Visibility.Collapsed;
            }
        }

        private void btnAbandonarPartida_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Estás seguro que deseas abandonar la partida? Perderás puntos de clasificación.",
                                                      "Abandonar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        private void btnJuicio_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string decision = btn.Name == "btnCorrecto" ? "Correcto" : "Incorrecto";

            MessageBox.Show($"Has marcado la letra como {decision}", "Juicio", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEnviarChat_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = txtChatMensaje.Text;
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                txtChatHistorial.Text += $"\nYo: {mensaje}";
                txtChatMensaje.Clear();
            }
        }

        private void txtChatHistorial_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtChatHistorial.ScrollToEnd();
        }
    }
}
