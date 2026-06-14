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
                        // Transformamos el DTO en un formato amigable para la interfaz
                        HistorialUI item = new HistorialUI
                        {
                            PalabraObjetivo = h.palabra,
                            NombreContrincante = h.usuarioContrincante,
                            FechaPartida = h.fechaPartida
                        };

                        // Determinamos el resultado basado en los puntos y el estado
                        if (h.puntos > 0)
                        {
                            item.EstadoResultado = "Victoria";
                            item.PuntosObtenidos = $"+{h.puntos}";
                            item.ColorResultado = "Green";
                            contadorVictorias++;
                        }
                        else if (h.puntos < 0)
                        {
                            item.EstadoResultado = "Abandonaste";
                            item.PuntosObtenidos = $"{h.puntos}";
                            item.ColorResultado = "Red";
                            contadorDerrotas++;
                        }
                        else
                        {
                            // Si los puntos son 0, verificamos si fue una derrota legítima o el rival huyó
                            if (h.estadoPartida == 5 || h.estadoPartida == 6)
                            {
                                item.EstadoResultado = "Abandonó";
                                item.PuntosObtenidos = "0";
                                item.ColorResultado = "Gray";
                            }
                            else
                            {
                                item.EstadoResultado = "Derrota";
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
                MessageBox.Show($"Error al cargar el historial: {ex.Message}", "Fallo de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbFiltroHistorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            // Verificamos que la lista y el combobox ya estén inicializados
            if (_historialCompleto == null || cbFiltroHistorial.SelectedItem == null) return;

            string filtroSeleccionado = (cbFiltroHistorial.SelectedItem as ComboBoxItem).Content.ToString();

            if (filtroSeleccionado == "Todos")
            {
                // Mostramos todos, ordenados de los más recientes a los más antiguos
                lbHistorial.ItemsSource = _historialCompleto.OrderByDescending(h => h.FechaPartida).ToList();
            }
            else
            {
                // Filtramos por la palabra exacta
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
        public DateTime FechaPartida { get; set; }
        public string ColorResultado { get; set; }
    }
}
