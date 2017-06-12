using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Abm_Chofer;
using UberFrba.Mappings;

namespace UberFrba.Rendicion_Viajes
{

    public partial class RendicionViaje : Form
    {
        private int porcentaje = Config.porcentajeRendicion;

        public RendicionViaje()
        {
            InitializeComponent();
        }

        private void RendicionViaje_Load(object sender, EventArgs e)
        {
            this.dtpFecha.Value = Config.fecha;

            this.comboTurnos.DisplayMember = "tur_descripcion";
            this.comboTurnos.ValueMember = "tur_id";
            this.comboTurnos.DataSource = new BaseDeDatos().select_query("SELECT tur_id,tur_descripcion FROM OSNR.Turno WHERE tur_habilitado = 1");
            this.lblPorcentaje.Text = "Porcentaje a utilizar para la rendicion (config): " + porcentaje.ToString() + "%";
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            if (this.txtChofer.Text == null || txtChofer.Text == "")
            {
                MessageBox.Show("Falta ID Chofer");
                return;
            }
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("fecha", new DbTypedValue(this.dtpFecha.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("idTurno", new DbTypedValue(this.comboTurnos.SelectedValue.ToString(), SqlDbType.Decimal));
            campos.Add("idChofer", new DbTypedValue(this.txtChofer.Text, SqlDbType.Decimal));
            campos.Add("porcentaje", new DbTypedValue(porcentaje.ToString(), SqlDbType.Decimal));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            SpExec sp = new SpExec(new BaseDeDatos(), "OSNR.CrearRendicion", campos, errorMensaje, null);
            dataGridView1.DataSource = sp.ExecAndGetDataTable();
            if (!sp.huboError())
                cargarDatosRendicion();
            else
                dataGridView2.DataSource = null;
        }

        private void cargarDatosRendicion()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("fecha", new DbTypedValue(this.dtpFecha.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("idTurno", new DbTypedValue(this.comboTurnos.SelectedValue.ToString(), SqlDbType.Decimal));
            campos.Add("idChofer", new DbTypedValue(this.txtChofer.Text, SqlDbType.Decimal));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView2.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.ObtenerRendicion", campos, errorMensaje);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbmChofer busquedaChofer = new AbmChofer(true);
            busquedaChofer.ShowDialog();
            this.txtChofer.Text = busquedaChofer.idChoferSeleccionado;
        }
    }
}
