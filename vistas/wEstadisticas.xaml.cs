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
    public partial class wEstadisticas : Page
    {
        // Guardamos la lista completa para no tener que llamar al servidor cada vez que usemos el filtro
        private List<HistorialUI> _historialCompleto = new List<HistorialUI>();

        public wEstadisticas()
        {
            InitializeComponent();
            CargarPuntajeGlobal();
            CargarHistorial();
        }

        private void CargarPuntajeGlobal()
        {
            int puntos = utils.Sesion.Instancia.Puntos;
            lbPuntajeGlobal.Text = $"{puntos} pts";
        }

        private void CargarHistorial()
        {
            int idJugador = utils.Sesion.Instancia.IdJugador;
            var estSrv = new EstadisticasServiceRef.EstadisticasServiceClient();

            try
            {
                var historialDTO = estSrv.ObtenerHistorial(idJugador);

                if (historialDTO != null)
                {
                    int contadorVictorias = 0;
                    int contadorDerrotas = 0;

                    foreach (var h in historialDTO)
                    {
                        bool esEspanol = utils.Sesion.Instancia.IdIdioma == 1;
                        string formatoFecha = esEspanol ? "dd/MM/yyyy hh:mm tt" : "MM/dd/yyyy hh:mm tt";

                        HistorialUI item = new HistorialUI
                        {
                            PalabraObjetivo = h.palabra,
                            NombreContrincante = h.usuarioContrincante,
                            FechaPartida = h.fechaPartida,
                            FechaPartidaStr = h.fechaPartida.ToString(formatoFecha) // Asignamos la cadena limpia
                        };

                        if (h.puntos > 0)
                        {
                            item.EstadoResultado = Properties.Resources.filtroVictoria;
                            item.PuntosObtenidos = $"+{h.puntos}";
                            item.ColorResultado = "Green";
                            contadorVictorias++;
                        }
                        else if (h.puntos < 0)
                        {
                            item.EstadoResultado = Properties.Resources.filtroAbandonaste;
                            item.PuntosObtenidos = $"{h.puntos}";
                            item.ColorResultado = "Red";
                            contadorDerrotas++;
                        }
                        else
                        {
                            if (h.estadoPartida == 5 || h.estadoPartida == 6)
                            {
                                item.EstadoResultado = Properties.Resources.filtroAbandono;
                                item.PuntosObtenidos = "0";
                                item.ColorResultado = "Gray";
                            }
                            else
                            {
                                item.EstadoResultado = Properties.Resources.filtroDerrota;
                                item.PuntosObtenidos = "0";
                                item.ColorResultado = "Orange";
                                contadorDerrotas++;
                            }
                        }

                        _historialCompleto.Add(item);
                    }

                    lbVictorias.Text = contadorVictorias.ToString();
                    lbDerrotas.Text = contadorDerrotas.ToString();
                    lbTotalPartidas.Text = _historialCompleto.Count.ToString();

                    AplicarFiltro();
                }

                estSrv.Close();
            }
            catch (Exception ex)
            {
                estSrv.Abort();
                MessageBox.Show(string.Format(Properties.Resources.msgErrorCargarHistorial, ex.Message),
                                Properties.Resources.titFalloConexion,
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbFiltroHistorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Esta línea evita el crash silencioso al cargar la ventana
            if (!this.IsLoaded) return;
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            if (_historialCompleto == null || cbFiltroHistorial.SelectedItem == null) return;

            string filtroSeleccionado = (cbFiltroHistorial.SelectedItem as ComboBoxItem).Content.ToString();

            // COMPARAMOS USANDO LA LLAVE DE RECURSO, NO LA PALABRA "Todos"
            if (filtroSeleccionado == Properties.Resources.filtroTodos)
            {
                lbHistorial.ItemsSource = _historialCompleto.OrderByDescending(h => h.FechaPartida).ToList();
            }
            else
            {
                // La lista se filtra automáticamente porque item.EstadoResultado 
                // y filtroSeleccionado ahora provienen exactamente del mismo archivo de recursos.
                var listaFiltrada = _historialCompleto
                                    .Where(h => h.EstadoResultado == filtroSeleccionado)
                                    .OrderByDescending(h => h.FechaPartida)
                                    .ToList();

                lbHistorial.ItemsSource = listaFiltrada;
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
                NavigationService.Navigate(new wPerfil());
            }
        }
    }



    public class HistorialUI
    {
        public string PalabraObjetivo { get; set; }
        public string EstadoResultado { get; set; }
        public string PuntosObtenidos { get; set; }
        public string NombreContrincante { get; set; }
        public DateTime FechaPartida { get; set; } // Se queda intacta para ordenar
        public string FechaPartidaStr { get; set; } // NUEVA: Se usa para mostrar en pantalla
        public string ColorResultado { get; set; }
    }
}
