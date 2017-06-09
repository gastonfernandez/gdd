using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UberFrba.Listado_Estadistico
{
    public partial class ListadoEstadistico : Form
    {

        private Dictionary<int, String> mapIndicesNombresSP = new Dictionary<int, string>();
        //private Dictionary<String,BaseDeDatos.ValorTipo> datosListadoActual = new Dictionary<String,gdDataBase.ValorTipo>();
        int indiceListadoActual;

        public ListadoEstadistico()
        {
            InitializeComponent();
            var yearList = Enumerable.Range(DateTime.Today.Year - 15, 20).ToList();
            yearList.Reverse();
            comboBoxAño.DataSource = yearList;
            comboBoxAño.SelectedIndex = 2;

            mapIndicesNombresSP.Add(0,"[ÑUFLO].TOP5ChoferesConMayorRecaudacion");
            mapIndicesNombresSP.Add(1,"[Ñuflo].TOP5ChoferesConViajeMasLargo");
            mapIndicesNombresSP.Add(2,"[ÑUFLO].TOP5ClientesConMayorConsumo");
            mapIndicesNombresSP.Add(3,"[ÑUFLO].TOP5ClientesConMayorCantidadDeMismoAutomovil");
            comboBoxListado.SelectedIndex = 0;
            
        //    datosListadoActual.Add("id",new gdDataBase.ValorTipo());
         //   datosListadoActual.Add("fecha_inicio",new gdDataBase.ValorTipo());
          //  datosListadoActual.Add("fecha_fin", new gdDataBase.ValorTipo());
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private DateTime calcularFecha(int anio, int semestre)
        {
            return new DateTime(anio, semestre * 6 + 1, 1);
        }

        private int semestre()
        {
            if (radioButtonQ1.Checked) return 0;
            else return 1;
        }

        private int anio() {
            return Convert.ToInt32(comboBoxAño.SelectedValue);
        }

        private DateTime fechaInicial(){
            return calcularFecha(this.anio(), this.semestre());
        }
        private DateTime fechaFinal()
        {
            return fechaInicial().AddMonths(6);
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
         /*   dataGridView1.DataSource = null;
            var fecha_inicio = fechaInicial().Date.ToString("yyyy-MM-dd HH:mm:ss.000");
            var fecha_fin = fechaFinal().Date.ToString("yyyy-MM-dd HH:mm:ss.000");
            datosListadoActual["fecha_inicio"] = new gdDataBase.ValorTipo(fecha_inicio, SqlDbType.DateTime);
            datosListadoActual["fecha_fin"] = new gdDataBase.ValorTipo(fecha_fin, SqlDbType.DateTime);
            indiceListadoActual = comboBoxListado.SelectedIndex;
            var nombre_sp = mapIndicesNombresTop5[comboBoxListado.SelectedIndex];
            Dictionary<String, gdDataBase.ValorTipo> camposValores = new Dictionary<string, gdDataBase.ValorTipo>();
            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();

            camposValores.Add("fecha_inicio", new gdDataBase.ValorTipo(fecha_inicio, SqlDbType.DateTime));
            camposValores.Add("fecha_fin", new gdDataBase.ValorTipo(fecha_fin, SqlDbType.DateTime));
            errorMensaje.Add(2627, "Ingresó una matrícula de aeronave ya registrada. Intente nuevamente...");

            dataGridViewListado.DataSource = new gdDataBase().ExecAndGetData(nombre_sp, camposValores, errorMensaje);

            this.dataGridViewListado_RowEnter(sender, new DataGridViewCellEventArgs(0,0));
            */
        }
         

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void actualizarDetalle(DataGridViewCellEventArgs e)
        {/*
            DataGridViewRow filaSeleccionada;
            if (dataGridViewListado.Rows.Count == 0) return;
            filaSeleccionada = dataGridViewListado.Rows[e.RowIndex];
            datosListadoActual["id"] = new gdDataBase.ValorTipo(filaSeleccionada.Cells[0].Value, SqlDbType.NVarChar);
            var camposValores = gdDataBase.newParameters();
            camposValores.Add("id", datosListadoActual["id"]);
            camposValores.Add("fecha_inicio", datosListadoActual["fecha_inicio"]);
            camposValores.Add("fecha_fin", datosListadoActual["fecha_fin"]);
            dataGridView1.DataSource = new gdDataBase().ExecAndGetData(mapIndicesDetallesTop5[indiceListadoActual], camposValores); 
          * */
        }

        private void dataGridViewListado_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            actualizarDetalle(e);
        }

        private void comboBoxListado_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }

        private void FormListadoEstadistico_Load(object sender, EventArgs e)
        {

        }


    }
}
