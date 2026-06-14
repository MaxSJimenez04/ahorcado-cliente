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
            MessageBoxResult result = MessageBox.Show(Properties.Resources.msgConfirmarAbandono,
                                                      Properties.Resources.titAbandonar,
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        private void btnJuicio_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string decision = btn.Name == "btnCorrecto" ? Properties.Resources.textCorrecto : Properties.Resources.textIncorrecto;

            MessageBox.Show(string.Format(Properties.Resources.msgJuicioLetra, decision),
                            Properties.Resources.titJuicio,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void btnEnviarChat_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = txtChatMensaje.Text;
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                txtChatHistorial.Text += string.Format(Properties.Resources.textYoChat, mensaje);
                txtChatMensaje.Clear();
            }
        }

        private void txtChatHistorial_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtChatHistorial.ScrollToEnd();
        }
    }
}
