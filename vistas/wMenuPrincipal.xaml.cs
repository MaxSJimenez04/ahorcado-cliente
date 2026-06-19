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
    public partial class wMenuPrincipal : Page
    {
        public wMenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnCrearPartida_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wCrearPartida());
        }

        private void btnUnirsePartida_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wUnirsePartida());
        }

        private void btnMiPerfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wPerfil());
        }

        private void btnOpciones_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wConfiguracion());
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            string usuarioActual = utils.Sesion.Instancia.Usuario;

            utils.Sesion.Instancia.CerrarSesion();

            if (!string.IsNullOrEmpty(usuarioActual))
            {
                try
                {
                    var sesionSrv = new SesionServiceRef.SesionServiceClient();

                    sesionSrv.CerrarSesion(usuarioActual); 

                    sesionSrv.Close();
                }
                catch (Exception)
                {
                }
            }

            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigate(new wInicio());
        }

        private void btnClasificacion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new wClasificacion());
        }
    }
}
