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
    public partial class wPerfil : Page
    {
        public wPerfil()
        {
            InitializeComponent();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wMenuPrincipal());
        }

        private void btnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wEstadisticas());
        }

        private void btnEditarPerfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wEditarPerfil());
        }
    }
}
