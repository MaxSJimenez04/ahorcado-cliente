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
    // Ya NO implementa el callback directamente: se suscribe a los eventos de la
    // conexión compartida (ConexionPartida), que es quien recibe los callbacks WCF.
    public partial class wPartidaJugador : Page
    {
        private bool _esJuez;
        private PartidaServiceRef.PartidaDTO _partidaActual;
        private utils.ConexionPartida _conexion;
        private utils.ChatCliente _chat;

        public wPartidaJugador(PartidaServiceRef.PartidaDTO partida, bool esJuez = false)
        {
            InitializeComponent();
            _partidaActual = partida;
            _esJuez = esJuez;
            
            _conexion = utils.ConexionPartida.Instancia;

            ConfigurarInterfazPorRol();
            CargarDatosIniciales();
            SuscribirEventos();
            ConectarChat();
        }

        // Nos enganchamos a los eventos del juego que dispara la conexión compartida.
        private void SuscribirEventos()
        {
            _conexion.LetraParaJuzgar += OnLetraParaJuzgar;
            _conexion.LetraPropuesta += OnLetraPropuesta;
            _conexion.ErrorJuicio += OnErrorJuicio;
            _conexion.FinPartida += OnFinPartida;
        }

        private void DesuscribirEventos()
        {
            _conexion.LetraParaJuzgar -= OnLetraParaJuzgar;
            _conexion.LetraPropuesta -= OnLetraPropuesta;
            _conexion.ErrorJuicio -= OnErrorJuicio;
            _conexion.FinPartida -= OnFinPartida;
        }

        private void CargarDatosIniciales()
        {
            txtCategoria.Text = string.Format(Properties.Resources.textCategoria, _partidaActual.categoriaPalabra);
            txtPista.Text = _partidaActual.descripcionPalabra;

            ActualizarAhorcado(_partidaActual.intentosFallidos);
            ActualizarCeldasPalabra(_partidaActual.progresoPalabra);

            if (_partidaActual.letrasUsadas != null)
            {
                foreach (char letra in _partidaActual.letrasUsadas)
                {
                    DesactivarBotonTeclado(letra);
                }
            }
        }

        private void ConfigurarInterfazPorRol()
        {
            if (_esJuez)
            {
                panelAdivinador.Visibility = Visibility.Collapsed;
                panelJuez.Visibility = Visibility.Visible;

                txtLetraJuez.Text = "-";
                btnCorrecto.IsEnabled = false;
                btnIncorrecto.IsEnabled = false;
            }
            else
            {
                panelAdivinador.Visibility = Visibility.Visible;
                panelJuez.Visibility = Visibility.Collapsed;
            }
        }

        private void ActualizarCeldasPalabra(char[] progresoPalabra)
        {
            var listaCeldas = new List<LetraCeldaUI>();

            foreach (char c in progresoPalabra)
            {
                listaCeldas.Add(new LetraCeldaUI
                {
                    Letra = c == '_' ? "" : c.ToString(),
                    ColorLetra = "#1E1E1E"
                });
            }

            icPalabra.ItemsSource = null;
            icPalabra.ItemsSource = listaCeldas;
        }

        private void ActualizarAhorcado(int fallos)
        {
            if (fallos > 6) fallos = 6;
            string rutaImagen = $"../resources/img/ahorcado/ahorcado_{fallos}.png";
            imgAhorcado.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
        }

        // ==========================================
        // ACCIONES DEL JUGADOR
        // ==========================================

        private void btnLetraTeclado_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            char letraSeleccionada = btn.Content.ToString()[0];

            try
            {
                int idJugadorActual = utils.Sesion.Instancia.IdJugador;
                _conexion.Cliente.ProponerLetra(_partidaActual.idPartida, idJugadorActual, letraSeleccionada);

                DesactivarBotonTeclado(letraSeleccionada);

                panelAdivinador.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.msgErrorEnviarLetra, ex.Message),
                                Properties.Resources.titDesconexion, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnJuicio_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            bool decisionEsCorrecta = btn.Name == "btnCorrecto";

            try
            {
                int idJugadorActual = utils.Sesion.Instancia.IdJugador;
                _conexion.Cliente.JuzgarLetra(_partidaActual.idPartida, idJugadorActual, decisionEsCorrecta);

                btnCorrecto.IsEnabled = false;
                btnIncorrecto.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.msgErrorEnviarVeredicto, ex.Message),
                                Properties.Resources.titDesconexion, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DesactivarBotonTeclado(char letra)
        {
            foreach (StackPanel fila in panelAdivinador.Children.OfType<StackPanel>())
            {
                foreach (Button btn in fila.Children.OfType<Button>())
                {
                    if (btn.Content.ToString() == letra.ToString())
                    {
                        btn.IsEnabled = false;

                        btn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A4A4A"));

                        return;
                    }
                }
            }
        }

        private void btnAbandonarPartida_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(Properties.Resources.msgConfirmarAbandono,
                                                      Properties.Resources.titAbandonar,
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int idJugadorActual = utils.Sesion.Instancia.IdJugador;
                    _conexion.Cliente.AbandonarPartida(_partidaActual.idPartida, idJugadorActual);
                }
                catch (Exception)
                {
                    // Si falla, igual cerramos abajo.
                }
                finally
                {
                    DesuscribirEventos();
                    _conexion.Cerrar();
                    _chat?.Desconectar();
                }

                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        // ==========================================
        // EVENTOS EN TIEMPO REAL (vienen de ConexionPartida)
        // ==========================================

        private void OnLetraParaJuzgar(char letra)
        {
            Dispatcher.Invoke(() =>
            {
                txtLetraJuez.Text = letra.ToString();
                txtLetraJuez.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E"));
                btnCorrecto.IsEnabled = true;
                btnIncorrecto.IsEnabled = true;
            });
        }

        private void OnLetraPropuesta(char letra, bool esCorrecta, char[] progresoPalabra, int intentosFallidos)
        {
            Dispatcher.Invoke(() =>
            {
                ActualizarCeldasPalabra(progresoPalabra);
                ActualizarAhorcado(intentosFallidos);
                DesactivarBotonTeclado(letra);

                // Desbloqueamos el teclado para el siguiente turno.
                // WPF mantendrá deshabilitados los botones que pasaron por DesactivarBotonTeclado()
                panelAdivinador.IsEnabled = true;

                if (_esJuez)
                {
                    txtLetraJuez.Foreground = esCorrecta
                        ? new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#28A745"))
                        : new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC3545"));
                }
            });
        }

        private void OnErrorJuicio(char letra, bool eraCorrecta)
        {
            Dispatcher.Invoke(() =>
            {
                btnCorrecto.IsEnabled = true;
                btnIncorrecto.IsEnabled = true;

                string mensajeError = eraCorrecta ? string.Format(Properties.Resources.msgErrorJuicioSi, letra)
                                                  : string.Format(Properties.Resources.msgErrorJuicioNo, letra);

                mensajeError = mensajeError.Replace("\\n", "\n");

                MessageBox.Show(mensajeError, Properties.Resources.titVeredictoDenegado, MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        private void OnFinPartida(int estadoFinal)
        {
            // Cambiamos a InvokeAsync para permitir pausas sin congelar la pantalla
            Dispatcher.InvokeAsync(async () =>
            {
                // LA LÍNEA CLAVE: Le damos a WPF 500 milisegundos para dibujar la última jugada
                await Task.Delay(500);
                string mensaje = "";
                string titulo = Properties.Resources.titPartidaFinalizada;

                switch (estadoFinal)
                {
                    case 3:
                        mensaje = _esJuez ? Properties.Resources.msgJuezDerrota : Properties.Resources.msgAdivinadorVictoria;
                        break;
                    case 4:
                        mensaje = _esJuez ? Properties.Resources.msgJuezVictoria : Properties.Resources.msgAdivinadorDerrota;
                        break;
                    case 5:
                        mensaje = _esJuez ? Properties.Resources.msgAbandonoPropio : Properties.Resources.msgAbandonoRivalJuez;
                        break;
                    case 6:
                        mensaje = _esJuez ? Properties.Resources.msgAbandonoRivalAdiv : Properties.Resources.msgAbandonoPropio;
                        break;
                }

                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Information);

                // Soltar los eventos y cerrar la conexión al terminar la partida.
                DesuscribirEventos();
                _conexion.Cerrar();
                _chat?.Desconectar();

                NavigationService.Navigate(new wMenuPrincipal());
            });
        }

        // ==========================================
        // LÓGICA DEL CHAT (sigue local por ahora; el socket se conecta aparte)
        // ==========================================

        private async void ConectarChat()
        {
            _chat = new utils.ChatCliente();
            _chat.MensajeRecibido += OnMensajeChatRecibido;

            try
            {
                // ⚠️ Cambia "127.0.0.1" por la IP de la máquina servidor para la prueba en red.
                await _chat.ConectarAsync("127.0.0.1", 9000, _partidaActual.idPartida, utils.Sesion.Instancia.Usuario);
            }
            catch (Exception)
            {
                // Si el servidor de chat no está corriendo, el juego sigue funcionando sin chat.
            }
        }

        private void OnMensajeChatRecibido(string usuario, string texto)
        {
            // Llega desde un hilo de fondo: saltar al hilo de UI para tocar el TextBox.
            Dispatcher.Invoke(() =>
            {
                // 3. Usamos AppendText también al recibir
                txtChatHistorial.AppendText($"{usuario}: {texto}{Environment.NewLine}");
            });
        }

        private async void btnEnviarChat_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = txtChatMensaje.Text;
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                // 1. Formateamos directamente con el texto limpio del recurso
                string textoLocal = string.Format(Properties.Resources.textYoChat, mensaje);

                // 2. Usamos AppendText agregando el salto de línea universal de .NET
                txtChatHistorial.AppendText(textoLocal + Environment.NewLine);
                txtChatMensaje.Clear();

                if (_chat != null)
                {
                    await _chat.EnviarMensajeAsync(mensaje);
                }
            }
        }

        private void txtChatMensaje_KeyDown(object sender, KeyEventArgs e)
        {
            // Verificamos si la tecla presionada fue 'Enter'
            if (e.Key == Key.Enter)
            {
                // Llamamos al mismo método que ya hace todo el trabajo de enviar el chat
                btnEnviarChat_Click(null, null);
            }
        }

        private void txtChatHistorial_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtChatHistorial.ScrollToEnd();
        }
    }

    public class LetraCeldaUI
    {
        public string Letra { get; set; }
        public string ColorLetra { get; set; }
    }
}