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
using UberFrba.Mappings;

namespace UberFrba.Abm_Cliente
{
    public partial class AbmCliente : Form
    {

        Validacion v = new Validacion();

        public AbmCliente()
        {
            InitializeComponent();
            cargarClientes();
        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {
            cargarClientes();
        }

        private void cargarClientes()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("nombre", new DbTypedValue(txtNombre.Text, SqlDbType.VarChar));
            campos.Add("apellido", new DbTypedValue(txtApellido.Text, SqlDbType.VarChar));
            campos.Add("dni", new DbTypedValue(txtDni.Text, SqlDbType.VarChar));
            
            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView1.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.BuscarClientes", campos, errorMensaje);
            habilitarBotones();
        }

        private void habilitarBotones()
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
            }
            else
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            FormEditarCliente form = new FormEditarCliente();
            form.Tag = "Editar";
            form.cargarDatos(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarClientes();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarClientes();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            
            DialogResult dialogResult = MessageBox.Show("Está seguro que desea eliminar al Cliente", "Uber", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
                campos.Add("clienteId", new DbTypedValue(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), SqlDbType.Decimal));
   
                Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                new BaseDeDatos().ExecSP("OSNR.DeshabilitarCliente", campos, errorMensaje);
               
                MessageBox.Show("Cliente deshabilitado exitosamente");
                cargarClientes();
            }            
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            habilitarBotones();
        }

        private void btnAñadir_Click(object sender, EventArgs e)
        {
            FormEditarCliente form = new FormEditarCliente();
            form.Tag = "Agregar";
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarClientes();
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

    }
}
