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
using System.ServiceModel;

namespace ClienteAhorcado.vistas
{
    public partial class wPartidaJugador : Page, PartidaServiceRef.IPartidaServiceCallback
    {
        private bool _esJuez;
        private PartidaServiceRef.PartidaDTO _partidaActual;
        private PartidaServiceRef.PartidaServiceClient _partidaCliente;

        public wPartidaJugador(PartidaServiceRef.PartidaDTO partida, bool esJuez = false)
        {
            InitializeComponent();
            _partidaActual = partida;
            _esJuez = esJuez;

            ConfigurarInterfazPorRol();
            CargarDatosIniciales();
            ConectarCanalDuplex();
        }

        private void ConectarCanalDuplex()
        {
            try
            {
                var contexto = new InstanceContext(this);
                _partidaCliente = new PartidaServiceRef.PartidaServiceClient(contexto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.msgErrorConectarServidor, ex.Message),
                                Properties.Resources.titFalloConexion, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarDatosIniciales()
        {
            txtCategoria.Text = string.Format(Properties.Resources.textCategoria, _partidaActual.nombrePartida);
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
                _partidaCliente.ProponerLetra(_partidaActual.idPartida, idJugadorActual, letraSeleccionada);

                btn.IsEnabled = false;
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
                _partidaCliente.JuzgarLetra(_partidaActual.idPartida, idJugadorActual, decisionEsCorrecta);

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
                    _partidaCliente.AbandonarPartida(_partidaActual.idPartida, idJugadorActual);
                    _partidaCliente.Close();
                }
                catch (Exception)
                {
                    _partidaCliente?.Abort();
                }

                NavigationService.Navigate(new wMenuPrincipal());
            }
        }

        // ==========================================
        // EVENTOS EN TIEMPO REAL (CALLBACKS WCF)
        // ==========================================

        public void NotificarLetraParaJuzgar(char letra)
        {
            Dispatcher.Invoke(() =>
            {
                txtLetraJuez.Text = letra.ToString();
                txtLetraJuez.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E"));
                btnCorrecto.IsEnabled = true;
                btnIncorrecto.IsEnabled = true;
            });
        }

        public void NotificarLetraPropuesta(char letra, bool esCorrecta, char[] progresoPalabra, int intentosFallidos)
        {
            Dispatcher.Invoke(() =>
            {
                ActualizarCeldasPalabra(progresoPalabra);
                ActualizarAhorcado(intentosFallidos);
                DesactivarBotonTeclado(letra);

                if (_esJuez)
                {
                    txtLetraJuez.Foreground = esCorrecta
                        ? new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#28A745"))
                        : new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC3545"));
                }
            });
        }

        public void NotificarErrorJuicio(char letra, bool eraCorrecta)
        {
            Dispatcher.Invoke(() =>
            {
                btnCorrecto.IsEnabled = true;
                btnIncorrecto.IsEnabled = true;

                string mensajeError = eraCorrecta ? string.Format(Properties.Resources.msgErrorJuicioSi, letra)
                                                  : string.Format(Properties.Resources.msgErrorJuicioNo, letra);

                MessageBox.Show(mensajeError, Properties.Resources.titVeredictoDenegado, MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public void NotificarFinPartida(int estadoFinal)
        {
            Dispatcher.Invoke(() =>
            {
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

                try { _partidaCliente.Close(); } catch { _partidaCliente?.Abort(); }
                NavigationService.Navigate(new wMenuPrincipal());
            });
        }

        public void NotificarJugadorUnido(PartidaServiceRef.PartidaDTO partida) { }

        // ==========================================
        // LÓGICA DEL CHAT
        // ==========================================
        private void btnEnviarChat_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = txtChatMensaje.Text;
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                txtChatHistorial.Text += string.Format(Properties.Resources.textYoChat, mensaje);
                txtChatMensaje.Clear();
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