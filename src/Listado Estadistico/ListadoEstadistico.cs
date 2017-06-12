using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Mappings;

namespace UberFrba.Listado_Estadistico
{
    public partial class ListadoEstadistico : Form
    {

        private Dictionary<int, String> mapIndicesNombresSP = new Dictionary<int, string>();
        private Dictionary<String, DbTypedValue> filtroFechas = new Dictionary<String, DbTypedValue>();

        public ListadoEstadistico()
        {
            InitializeComponent();
            var yearList = Enumerable.Range(DateTime.Today.Year - 15, 20).ToList();
            yearList.Reverse();
            comboBoxAño.DataSource = yearList;
            comboBoxAño.SelectedIndex = 6;

            mapIndicesNombresSP.Add(0, "[OSNR].TOP5ChoferesConMayorRecaudacion");
            mapIndicesNombresSP.Add(1, "[OSNR].TOP5ChoferesConViajeMasLargo");
            mapIndicesNombresSP.Add(2, "[OSNR].TOP5ClientesConMayorConsumo");
            mapIndicesNombresSP.Add(3, "[OSNR].TOP5ClientesConMayorCantidadDeMismoAutomovil");
            comboBoxListado.SelectedIndex = 0;

            filtroFechas.Add("fecha_inicio", null);
            filtroFechas.Add("fecha_fin", null);
        }

        private DateTime calcularFecha(int anio, int trimestre)
        {
            return new DateTime(anio, (trimestre - 1) * 3 + 1, 1);
        }

        private int trimestre()
        {
            if (radioButtonQ1.Checked) return 1;
            if (radioButtonQ2.Checked) return 2;
            if (radioButtonQ3.Checked) return 3;
            if (radioButtonQ4.Checked) return 4;
            throw new Exception("Seleccione Trimestre");
        }

        private int anio()
        {
            return Convert.ToInt32(comboBoxAño.SelectedValue);
        }

        private DateTime fechaInicial()
        {
            return calcularFecha(this.anio(), this.trimestre());
        }

        private DateTime fechaFinal()
        {
            return fechaInicial().AddMonths(3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            var fecha_inicio = fechaInicial().Date.ToString("yyyy-MM-dd HH:mm:ss.000");
            var fecha_fin = fechaFinal().Date.ToString("yyyy-MM-dd HH:mm:ss.000");
            filtroFechas["fecha_inicio"] = new DbTypedValue(fecha_inicio, SqlDbType.DateTime);
            filtroFechas["fecha_fin"] = new DbTypedValue(fecha_fin, SqlDbType.DateTime);
            var nombre_sp = mapIndicesNombresSP[comboBoxListado.SelectedIndex];

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView1.DataSource = new BaseDeDatos().ExecSPAndGetData(nombre_sp, filtroFechas, errorMensaje);

        }
         
        private void comboBoxListado_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }

    }
}
