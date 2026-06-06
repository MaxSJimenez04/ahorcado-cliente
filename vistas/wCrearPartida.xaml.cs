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
    public partial class wCrearPartida : Page
    {
        public wCrearPartida()
        {
            InitializeComponent();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            VolverAtras();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VolverAtras();
        }

        private void VolverAtras()
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

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            string nombrePartida = txtNombrePartida.Text;

            var categoriaSeleccionada = cbCategoria.SelectedItem as ComboBoxItem;
            var palabraSeleccionada = lbPalabras.SelectedItem as ListBoxItem;

            if (string.IsNullOrWhiteSpace(nombrePartida) || categoriaSeleccionada == null || palabraSeleccionada == null)
            {
                MessageBox.Show("Por favor, llena todos los campos y selecciona una palabra para la partida.",
                                "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Creando partida '{nombrePartida}'.\nPalabra a adivinar: {palabraSeleccionada.Content}",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new wEsperaJugador());
        }
    }
}
