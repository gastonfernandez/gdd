using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace UberFrba.Mappings
{
    class BaseDeDatos
    {

        public SqlConnection conexion = new SqlConnection(Config.strConnection);

        public void openConnection() { conexion.Open(); }

        public void closeConnection() { conexion.Close(); }

        public void query(String query)
        {
            try
            {
                conexion.Open();
                SqlCommand queryCommand = new SqlCommand(query, conexion);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                conexion.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Query: " + query);
            }
        }

        public DataTable select_query(String query)
        {
            try
            {
                conexion.Open();
                SqlCommand queryCommand = new SqlCommand(query, conexion);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);
                conexion.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Query: " + query);
            }

            return new DataTable();
        }

        public DataTable ExecSPAndGetData(String spName, Dictionary<String, DbTypedValue> fields = null, Dictionary<int, String> errorMensaje = null, String ejecucionCorrecta = null)
        {
            return new SpExec(this, spName, fields, errorMensaje, ejecucionCorrecta).ExecAndGetDataTable();
        }

        public SpExec ExecSP(String spName, Dictionary<String, DbTypedValue> fields = null, Dictionary<int, String> errorMensaje = null, String ejecucionCorrecta = null)
        {
            return new SpExec(this, spName, fields, errorMensaje, ejecucionCorrecta).Exec();
        }
    }
}
