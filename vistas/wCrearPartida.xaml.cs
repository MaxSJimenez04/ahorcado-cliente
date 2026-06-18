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
    // Ya NO implementa IPartidaServiceCallback: quien escucha al servidor es
    // ConexionPartida. Esta pantalla solo recolecta datos y crea la partida.
    public partial class wCrearPartida : Page
    {
        public wCrearPartida()
        {
            InitializeComponent();
            CargarCategorias();
        }

        // ==========================================
        // CARGA DINÁMICA DE DATOS (PalabraService: NO es dúplex, queda igual)
        // ==========================================
        private void CargarCategorias()
        {
            var palabraSrv = new PalabraServiceRef.PalabraServiceClient();
            try
            {
                var categorias = palabraSrv.ObtenerCategorias();
                if (categorias != null && categorias.Length > 0)
                {

                    cbCategoria.DisplayMemberPath = utils.Sesion.Instancia.IdIdioma == 1 ? "categoriaES" : "categoriaEN";
                    cbCategoria.SelectedValuePath = "idCategoria";

                    cbCategoria.ItemsSource = categorias;
                    cbCategoria.SelectedIndex = 0;
                }
                palabraSrv.Close();
            }
            catch (Exception ex)
            {
                palabraSrv.Abort();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorCargarCategorias, ex.Message),
                                Properties.Resources.titErrorConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategoria.SelectedValue != null)
            {
                int idCategoria = (int)cbCategoria.SelectedValue;
                CargarPalabras(idCategoria);
            }
        }

        private void CargarPalabras(int idCategoria)
        {
            var palabraSrv = new PalabraServiceRef.PalabraServiceClient();
            try
            {
                var palabras = palabraSrv.ObtenerPalabrasPorCategoria(idCategoria);
                if (palabras != null)
                {
                    lbPalabras.ItemsSource = palabras;

                    lbPalabras.DisplayMemberPath = utils.Sesion.Instancia.IdIdioma == 1 ? "palabraES" : "palabraEN";
                    lbPalabras.SelectedValuePath = "idPalabra";
                }
                palabraSrv.Close();
            }
            catch (Exception ex)
            {
                palabraSrv.Abort();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorCargarPalabras, ex.Message),
                                Properties.Resources.titErrorConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // NAVEGACIÓN
        // ==========================================
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

        // ==========================================
        // CREACIÓN DE PARTIDA
        // ==========================================
        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            string nombrePartida = txtNombrePartida.Text;

            var idCategoriaObj = cbCategoria.SelectedValue;
            var idPalabraObj = lbPalabras.SelectedValue;

            if (string.IsNullOrWhiteSpace(nombrePartida) || idCategoriaObj == null || idPalabraObj == null)
            {
                MessageBox.Show(Properties.Resources.msgCamposPartidaIncompletos,
                                Properties.Resources.TitDatosIncompletos, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int idPalabra = (int)idPalabraObj;
            int idJugador = utils.Sesion.Instancia.IdJugador;
            int idIdioma = utils.Sesion.Instancia.IdIdioma;

            // Usamos la conexión COMPARTIDA. El servidor registra el callback en
            // ESTA conexión, y la misma seguirá viva en wEsperaJugador y wPartidaJugador.
            var partidaSrv = utils.ConexionPartida.Instancia.Conectar();

            try
            {
                int resultadoPartida = partidaSrv.CrearPartida(idJugador, idPalabra, nombrePartida, idIdioma);

                if (resultadoPartida > 0)
                {
                    // NO cerramos la conexión: debe seguir viva para recibir
                    // NotificarJugadorUnido en la pantalla de espera.
                    // Navegamos de inmediato para que wEsperaJugador se suscriba cuanto antes.
                    NavigationService.Navigate(new wEsperaJugador(resultadoPartida));
                }
                else if (resultadoPartida == -1)
                {
                    MessageBox.Show(Properties.Resources.msgNombrePartidaEnUso,
                                    Properties.Resources.titNombreOcupado, MessageBoxButton.OK, MessageBoxImage.Warning);
                    // No entramos a la partida: cerramos la conexión que abrimos.
                    utils.ConexionPartida.Instancia.Cerrar();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgErrorBDCrearPartida,
                                    Properties.Resources.titErrorServidor, MessageBoxButton.OK, MessageBoxImage.Error);
                    utils.ConexionPartida.Instancia.Cerrar();
                }
            }
            catch (Exception ex)
            {
                utils.ConexionPartida.Instancia.Cerrar();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorComunicarServidor, ex.Message),
                                Properties.Resources.titFalloConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}