using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UberFrba.Mappings
{
    class SpExec
    {
        protected String spName;
        protected Dictionary<String, DbTypedValue> fields;
        protected Dictionary<int, String> errorMensaje;
        protected String msgEjecucionCorrecta;
        protected SqlException excepcionAtrapada;
        protected BaseDeDatos db;

        public SpExec(BaseDeDatos db, String spName, Dictionary<String, DbTypedValue> fields = null, Dictionary<int, String> errorMensaje = null, String msgEjecucionCorrecta = null) 
        {
            this.spName = spName;
            this.fields = fields;
            this.errorMensaje = errorMensaje;
            this.msgEjecucionCorrecta = msgEjecucionCorrecta;
            this.excepcionAtrapada = null;
            this.db = db;
        }

        public void agregarParametrosAComando(SqlCommand cmd)
        {
            if (this.fields != null)
            {
                foreach (KeyValuePair<String, DbTypedValue> keyValue in fields)
                {
                    var param = new SqlParameter("@" + keyValue.Key, keyValue.Value.getType());
                    param.Precision = 18;
                    param.Scale = 0;
                    param.Value = keyValue.Value.getValue();
                    cmd.Parameters.Add(param);
                }
            }
        }

        public void mostrarErrorSqlProducido()
        {

            Boolean encontroErrorConocido = false;
            if (errorMensaje != null)
            {
                for (int i = 0; i < errorMensaje.Count; i++)
                {
                    if (excepcionAtrapada.Number == errorMensaje.ElementAt(i).Key)
                    {
                        if (excepcionAtrapada.Number >= 64000)
                            MessageBox.Show(errorMensaje.ElementAt(i).Value, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        else
                            MessageBox.Show(errorMensaje.ElementAt(i).Value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        encontroErrorConocido = true;
                    }
                }
            }
            if (!encontroErrorConocido)
                MessageBox.Show(excepcionAtrapada.Message);
        }

        public Boolean huboError() 
        {
            return excepcionAtrapada != null;
        }

        public int codError() 
        {
            if (excepcionAtrapada == null) throw new Exception("no habia ninguna excepcion");
            return excepcionAtrapada.Number;
        }

        protected void mostrarResultadoEjecucionCorrecta() {
            if (msgEjecucionCorrecta != null)
                MessageBox.Show(msgEjecucionCorrecta);
        }

        public void Exec()
        {
            db.openConnection();
            using (var cmd = new SqlCommand(spName, db.conexion))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                agregarParametrosAComando(cmd);

                try
                {
                    cmd.ExecuteNonQuery();
                    mostrarResultadoEjecucionCorrecta();
                }
                catch (SqlException exception)
                {
                    excepcionAtrapada = exception;
                    mostrarErrorSqlProducido();
                }
            }

            db.closeConnection();
        }

        public DataTable ExecAndGetDataTable()
        {
            db.openConnection();
            DataTable ds = new DataTable();

            using (var cmd = new SqlCommand(spName, db.conexion))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                agregarParametrosAComando(cmd);

                try
                {
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    mostrarResultadoEjecucionCorrecta();
                }
                catch (SqlException exception)
                {
                    excepcionAtrapada = exception;
                    mostrarErrorSqlProducido();
                }
            }

            db.closeConnection();
            return ds;
        }
        
    }
}
