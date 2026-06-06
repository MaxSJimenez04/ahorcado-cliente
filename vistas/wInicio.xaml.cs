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
    public partial class wInicio : Page
    {
        public wInicio()
        {
            InitializeComponent();
        }

        private void btnJugar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wLogin());
        }

        private void btnOpciones_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wConfiguracion());
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
