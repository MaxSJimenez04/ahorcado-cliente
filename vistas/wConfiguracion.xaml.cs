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
    /// <summary>
    /// Lógica de interacción para wConfiguracion.xaml
    /// </summary>
    public partial class wConfiguracion : Page
    {
        public wConfiguracion()
        {
            InitializeComponent();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si hay historial de navegación para regresar de forma natural
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                // Respaldo por si la página se abre directamente
                NavigationService.Navigate(new wInicio());
            }
        }
    }
}
