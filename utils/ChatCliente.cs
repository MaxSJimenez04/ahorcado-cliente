using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAhorcado.utils
{
    public class ChatCliente
    {
        private TcpClient _tcp;
        private StreamWriter _escritor;
        private StreamReader _lector;
        private int _idPartida;
        private string _usuario;

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

            await _escritor.WriteLineAsync($"JOIN|{_idPartida}|{_usuario}");

            _ = Task.Run(EscucharMensajes);
        }

        private async Task EscucharMensajes()
        {
            try
            {
                string linea;
                while ((linea = await _lector.ReadLineAsync()) != null)
                {
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