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
    public partial class wEditarPerfil : Page
    {
        private bool _contraseniaVisible = false;

        public wEditarPerfil()
        {
            InitializeComponent();
            dpFechaNacimiento.SelectedDate = new System.DateTime(1998, 5, 12);
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            RegresarAPerfil();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            RegresarAPerfil();
        }

        private void RegresarAPerfil()
        {
            NavigationService.Navigate(new wPerfil());
        }

        private void btnMostrarContrasenia_Click(object sender, RoutedEventArgs e)
        {
            _contraseniaVisible = !_contraseniaVisible;

            if (_contraseniaVisible)
            {
                txtContraseniaVisible.Text = pbContraseniaOculta.Password;
                pbContraseniaOculta.Visibility = Visibility.Collapsed;
                txtContraseniaVisible.Visibility = Visibility.Visible;
            }
            else
            {
                pbContraseniaOculta.Password = txtContraseniaVisible.Text;
                txtContraseniaVisible.Visibility = Visibility.Collapsed;
                pbContraseniaOculta.Visibility = Visibility.Visible;
            }
        }

        private void btnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cambios guardados con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            RegresarAPerfil();
        }
    }
}
