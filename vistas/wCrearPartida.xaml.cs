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
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            var palabraSrv = new PalabraServiceRef.PalabraServiceClient();
            try
            {
                var categorias = palabraSrv.ObtenerCategorias();
                if (categorias != null && categorias.Length > 0)
                {
                    bool esEspanol = utils.Sesion.Instancia.IdIdioma == 1;

                    var listaCategorias = categorias
                        .Select(c => new CategoriaItemUI
                        {
                            Id = c.idCategoria,
                            Nombre = esEspanol ? c.categoriaES : c.categoriaEN
                        })
                        .ToList();

                    cbCategoria.SelectedValuePath = "Id";
                    cbCategoria.ItemsSource = listaCategorias;
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

            var partidaSrv = utils.ConexionPartida.Instancia.Conectar();

            try
            {
                int resultadoPartida = partidaSrv.CrearPartida(idJugador, idPalabra, nombrePartida, idIdioma);

                if (resultadoPartida > 0)
                {
                    NavigationService.Navigate(new wEsperaJugador(resultadoPartida));
                }
                else if (resultadoPartida == -1)
                {
                    MessageBox.Show(Properties.Resources.msgNombrePartidaEnUso,
                                    Properties.Resources.titNombreOcupado, MessageBoxButton.OK, MessageBoxImage.Warning);
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

    public class CategoriaItemUI
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}