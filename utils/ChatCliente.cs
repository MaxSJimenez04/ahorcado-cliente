using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAhorcado.utils
{
    // Cliente de chat por socket TCP. Se conecta al ServidorChatAhorcado,
    // entra a la sala de su partida (JOIN), escucha mensajes entrantes y
    // permite enviar (MSG). Al salir manda LEAVE y cierra el socket.
    public class ChatCliente
    {
        private TcpClient _tcp;
        private StreamWriter _escritor;
        private StreamReader _lector;
        private int _idPartida;
        private string _usuario;

        // La ventana de partida se suscribe a esto para mostrar lo que llega del otro jugador.
        // (usuario, texto)
        public event Action<string, string> MensajeRecibido;

        public async Task ConectarAsync(string ipServidor, int puerto, int idPartida, string usuario)
        {
            _idPartida = idPartida;
            _usuario = usuario;

            _tcp = new TcpClient();
            await _tcp.ConnectAsync(ipServidor, puerto);

            var stream = _tcp.GetStream();
            _lector = new StreamReader(stream, Encoding.UTF8);
            _escritor = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Avisar al servidor que entramos a la sala de esta partida
            await _escritor.WriteLineAsync($"JOIN|{_idPartida}|{_usuario}");

            // Empezar a escuchar mensajes entrantes en segundo plano
            _ = Task.Run(EscucharMensajes);
        }

        private async Task EscucharMensajes()
        {
            try
            {
                string linea;
                while ((linea = await _lector.ReadLineAsync()) != null)
                {
                    // Formato esperado: MSG|idPartida|usuario|texto
                    string[] partes = linea.Split('|');
                    if (partes.Length >= 4 && partes[0] == "MSG")
                    {
                        string usuario = partes[2];
                        string texto = partes[3];
                        MensajeRecibido?.Invoke(usuario, texto);
                    }
                }
            }
            catch
            {
                // El socket se cerró (fin de partida o caída): se ignora.
            }
        }

        public async Task EnviarMensajeAsync(string texto)
        {
            if (_escritor == null) return;
            try
            {
                await _escritor.WriteLineAsync($"MSG|{_idPartida}|{_usuario}|{texto}");
            }
            catch
            {
                // Falló el envío
            }
        }

        public void Desconectar()
        {
            try
            {
                _escritor?.WriteLine($"LEAVE|{_idPartida}|{_usuario}");
            }
            catch { }
            finally
            {
                try { _tcp?.Close(); } catch { }
            }
        }
    }
}