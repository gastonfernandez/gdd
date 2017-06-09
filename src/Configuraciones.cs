using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace UberFrba
{
    class Configuraciones
    {
        public static FormLogin formLogin;
        public static String datosConexion = Properties.Settings.Default.GD1C2017ConnectionString;
        public static int cantMaxIntentosLogin = Properties.Settings.Default.intentosLogin;
        public static decimal usuario;
        public static decimal rol;
        public static DateTime fecha = Properties.Settings.Default.fecha;

        public static void validarCierreVentana(Object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms.Count == 1 && formLogin.Visible == false) System.Windows.Forms.Application.Exit();
        }
    }
}
