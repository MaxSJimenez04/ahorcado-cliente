using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAhorcado.utils
{
    public class Sesion
    {
        private static Sesion _instancia;

        public int IdJugador { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Puntos { get; set; }
        public int IdIdioma { get; set; } = 1;

        private Sesion() { }

        public static Sesion Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new Sesion();
                }
                return _instancia;
            }
        }

        public void CerrarSesion()
        {
            IdJugador = 0;
            Usuario = null;
            Nombre = null;
            PrimerApellido = null;
            SegundoApellido = null;
            Correo = null;
            Telefono = null;
            Puntos = 0;
            FechaNacimiento = DateTime.MinValue;
        }
    }
}
