using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Abm_Cliente;
using UberFrba.Mappings;

namespace UberFrba.Facturacion
{
    public partial class FacturacionCliente : Form
    {
        public FacturacionCliente()
        {
            InitializeComponent();
        }

        private void FacturacionCliente_Load(object sender, EventArgs e)
        {
            this.dtpFechaInicio.Value = Config.fecha;
            this.dtpFechaFin.Value = Config.fecha.AddDays(1);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            if (this.txtCliente.Text == null || txtCliente.Text == "")
            {
                MessageBox.Show("Falta ID Cliente");
                return;
            }
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("fechaInicio", new DbTypedValue(this.dtpFechaInicio.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("fechaFin", new DbTypedValue(this.dtpFechaFin.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("idCliente", new DbTypedValue(this.txtCliente.Text, SqlDbType.Decimal));
            campos.Add("hoy", new DbTypedValue(Config.fecha.ToString("yyyy-MM-dd"), SqlDbType.Date));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            SpExec sp = new SpExec(new BaseDeDatos(), "OSNR.CrearFactura", campos, errorMensaje, null);
            dataGridView1.DataSource = sp.ExecAndGetDataTable();
            if (!sp.huboError())
                cargarDatosFactura();
            else
                dataGridView2.DataSource = null;
        }

        private void cargarDatosFactura()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("fechaInicio", new DbTypedValue(this.dtpFechaInicio.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("fechaFin", new DbTypedValue(this.dtpFechaFin.Value.ToString("yyyy-MM-dd"), SqlDbType.Date));
            campos.Add("idCliente", new DbTypedValue(this.txtCliente.Text, SqlDbType.Decimal));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView2.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.ObtenerFactura", campos, errorMensaje);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbmCliente busquedaCliente = new AbmCliente(true);
            busquedaCliente.ShowDialog();
            this.txtCliente.Text = busquedaCliente.idClienteSeleccionado;
        }
    
    }
}
