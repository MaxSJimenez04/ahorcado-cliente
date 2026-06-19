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
using System.Threading;
using System.Globalization;

namespace ClienteAhorcado.vistas
{
    public partial class wConfiguracion : Page
    {
        public wConfiguracion()
        {
            InitializeComponent();
            CargarIdiomaActual();
        }

        private void CargarIdiomaActual()
        {
            if (utils.Sesion.Instancia.IdIdioma == 2)
            {
                cbIdioma.SelectedIndex = 1;
            }
            else
            {
                cbIdioma.SelectedIndex = 0; 
            }
        }

        private void cbIdioma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;

            int seleccion = cbIdioma.SelectedIndex;
            string codigoCultura = "es-MX";
            int nuevoIdIdioma = 1;

            if (seleccion == 1)
            {
                codigoCultura = "en-US";
                nuevoIdIdioma = 2;
            }

            if (utils.Sesion.Instancia.IdIdioma == nuevoIdIdioma) return;

            var culturaInfo = new CultureInfo(codigoCultura);
            Thread.CurrentThread.CurrentUICulture = culturaInfo;
            Thread.CurrentThread.CurrentCulture = culturaInfo;

            Properties.Resources.Culture = culturaInfo;

            utils.Sesion.Instancia.IdIdioma = nuevoIdIdioma;

            txtTituloOpciones.Text = Properties.Resources.btnOpciones;
            txtEtiquetaIdioma.Text = Properties.Resources.textIdioma;
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(utils.Sesion.Instancia.Usuario))
            {
                NavigationService.Navigate(new wMenuPrincipal());
            }
            else
            {
                NavigationService.Navigate(new wInicio());
            }
        }
    }
}