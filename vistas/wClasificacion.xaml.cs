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
    public partial class wClasificacion : Page
    {
        public wClasificacion()
        {
            InitializeComponent();
            CargarClasificacionGlobal();
            CargarPosicionPersonal();
        }

        private void CargarClasificacionGlobal()
        {
            var estSrv = new EstadisticasServiceRef.EstadisticasServiceClient();

            try
            {
                var top25 = estSrv.ObtenerClasificacionPuntos();

                if (top25 != null)
                {
                    List<ClasificacionUI> listaGrafica = new List<ClasificacionUI>();

                    foreach (var jugador in top25)
                    {
                        var itemUI = new ClasificacionUI
                        {
                            PosicionStr = $"{jugador.posicion}°",
                            Usuario = jugador.usuario,
                            PuntosStr = $"{jugador.puntos} pts"
                        };

                        switch (jugador.posicion)
                        {
                            case 1: itemUI.ColorMedalla = "#FADB5F"; break;
                            case 2: itemUI.ColorMedalla = "#E0E0E0"; break;
                            case 3: itemUI.ColorMedalla = "#ECA26C"; break;
                            default: itemUI.ColorMedalla = "#FFFFFF"; break;
                        }

                        listaGrafica.Add(itemUI);
                    }

                    lbClasificacion.ItemsSource = listaGrafica;
                }

                estSrv.Close();
            }
            catch (Exception ex)
            {
                estSrv.Abort();

                MessageBox.Show(string.Format(Properties.Resources.msgErrorClasificacion, ex.Message),
                                Properties.Resources.titFalloConexion,
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarPosicionPersonal()
        {
            int idJugadorActual = utils.Sesion.Instancia.IdJugador;
            var estSrv = new EstadisticasServiceRef.EstadisticasServiceClient();

            try
            {
                var miPosicion = estSrv.ObtenerEstadisticaUsuario(idJugadorActual, 1);

                if (miPosicion != null)
                {
                    txtMiPosicion.Text = $"{miPosicion.posicion}°";
                    txtMiUsuario.Text = miPosicion.usuario;
                    txtMisPuntos.Text = $"{miPosicion.puntos} pts";
                }
                else
                {
                    txtMiPosicion.Text = "-°";
                    txtMiUsuario.Text = utils.Sesion.Instancia.Usuario;
                    txtMisPuntos.Text = $"{utils.Sesion.Instancia.Puntos} pts";
                }

                estSrv.Close();
            }
            catch (Exception)
            {
                estSrv.Abort();
                txtMiPosicion.Text = "-°";
                txtMiUsuario.Text = utils.Sesion.Instancia.Usuario;
                txtMisPuntos.Text = $"{utils.Sesion.Instancia.Puntos} pts";
            }
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
    }



    public class ClasificacionUI
    {
        public string PosicionStr { get; set; }
        public string Usuario { get; set; }
        public string PuntosStr { get; set; }
        public string ColorMedalla { get; set; }
    }
}
