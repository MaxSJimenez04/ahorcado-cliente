using System;
using System.ServiceModel;
using ClienteAhorcado.PartidaServiceRef;

namespace ClienteAhorcado.utils
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ConexionPartida : IPartidaServiceCallback
    {
        private static ConexionPartida _instancia;
        public static ConexionPartida Instancia => _instancia ?? (_instancia = new ConexionPartida());

        public PartidaServiceClient Cliente { get; private set; }

        public event Action<PartidaDTO> JugadorUnido;
        public event Action<char> LetraParaJuzgar;
        public event Action<char, bool, char[], int> LetraPropuesta;
        public event Action<char, bool> ErrorJuicio;
        public event Action<int> FinPartida;

        private ConexionPartida() { }

        public PartidaServiceClient Conectar()
        {
            if (Cliente == null ||
                Cliente.State == CommunicationState.Faulted ||
                Cliente.State == CommunicationState.Closed)
            {
                var contexto = new InstanceContext(this);
                Cliente = new PartidaServiceClient(contexto);
            }
            return Cliente;
        }

        public void Cerrar()
        {
            try
            {
                if (Cliente != null &&
                    Cliente.State != CommunicationState.Faulted &&
                    Cliente.State != CommunicationState.Closed)
                    Cliente.Close();
            }
            catch { Cliente?.Abort(); }
            finally
            {
                Cliente = null;
                JugadorUnido = null; LetraParaJuzgar = null;
                LetraPropuesta = null; ErrorJuicio = null; FinPartida = null;
            }
        }

        public void NotificarJugadorUnido(PartidaDTO partida) => JugadorUnido?.Invoke(partida);
        public void NotificarLetraParaJuzgar(char letra) => LetraParaJuzgar?.Invoke(letra);
        public void NotificarLetraPropuesta(char letra, bool esCorrecta, char[] progreso, int fallidos)
            => LetraPropuesta?.Invoke(letra, esCorrecta, progreso, fallidos);
        public void NotificarErrorJuicio(char letra, bool eraCorrecta) => ErrorJuicio?.Invoke(letra, eraCorrecta);
        public void NotificarFinPartida(int estadoFinal) => FinPartida?.Invoke(estadoFinal);
    }
}