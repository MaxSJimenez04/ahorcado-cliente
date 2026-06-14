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
using System.ServiceModel; // Necesario para InstanceContext

namespace ClienteAhorcado.vistas
{
    // ¡OJO! Aquí agregamos la interfaz del callback generada por tu referencia de WCF
    public partial class wCrearPartida : Page
    {
        public wCrearPartida()
        {
            InitializeComponent();
            CargarCategorias();
        }

        // ==========================================
        // CARGA DINÁMICA DE DATOS
        // ==========================================
        private void CargarCategorias()
        {
            var palabraSrv = new PalabraServiceRef.PalabraServiceClient();
            try
            {
                var categorias = palabraSrv.ObtenerCategorias();
                if (categorias != null && categorias.Length > 0)
                {
                    cbCategoria.ItemsSource = categorias;

                    // Elegimos qué propiedad mostrar basándonos en el idioma de la sesión global
                    cbCategoria.DisplayMemberPath = utils.Sesion.Instancia.IdIdioma == 1 ? "categoriaES" : "categoriaEN";
                    cbCategoria.SelectedValuePath = "idCategoria";

                    cbCategoria.SelectedIndex = 0;
                }
                palabraSrv.Close();
            }
            catch (Exception ex)
            {
                palabraSrv.Abort();
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    // Mostramos la palabra correcta según el idioma
                    lbPalabras.DisplayMemberPath = utils.Sesion.Instancia.IdIdioma == 1 ? "palabraES" : "palabraEN";
                    lbPalabras.SelectedValuePath = "idPalabra";
                }
                palabraSrv.Close();
            }
            catch (Exception ex)
            {
                palabraSrv.Abort();
                MessageBox.Show($"Error al cargar palabras: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Por favor, llena todos los campos y selecciona una palabra para la partida.",
                                "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int idPalabra = (int)idPalabraObj;
            int idJugador = utils.Sesion.Instancia.IdJugador;
            int idIdioma = utils.Sesion.Instancia.IdIdioma;

            // 1. Crear el contexto de instancia apuntando a esta clase (que implementa los callbacks)
            var contexto = new InstanceContext(this);

            // 2. Crear el cliente Duplex usando el contexto
            var partidaSrv = new PartidaServiceRef.PartidaServiceClient(contexto);

            try
            {
                // 3. Llamar al método del servidor
                int resultadoPartida = partidaSrv.CrearPartida(idJugador, idPalabra, nombrePartida, idIdioma);

                if (resultadoPartida > 0)
                {
                    MessageBox.Show($"¡Partida '{nombrePartida}' creada con éxito!",
                                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new wEsperaJugador(resultadoPartida));
                }
                else if (resultadoPartida == -1)
                {
                    MessageBox.Show("El nombre de la partida ya está en uso. Por favor, elige otro.",
                                    "Nombre ocupado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    partidaSrv.Close();
                }
                else
                {
                    MessageBox.Show("Ocurrió un error en la base de datos al intentar crear la partida.",
                                    "Error del servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                    partidaSrv.Close();
                }
            }
            catch (Exception ex)
            {
                partidaSrv.Abort();
                MessageBox.Show($"Ocurrió un error al comunicar con el servidor: {ex.Message}",
                                "Fallo de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
