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

namespace UberFrba.Abm_Cliente
{
    public partial class FormCliente : Form
    {

        SqlConnection conexion;
        Validacion v = new Validacion();

        public FormCliente()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Config.strConnection);
        }

        private void cargarClientes()
        {
            conexion.Open();
            SqlCommand cargar = new SqlCommand("OSNR.CargarClientes", conexion);
            cargar.CommandType = CommandType.StoredProcedure;
            cargar.Parameters.Add("@Cliente", SqlDbType.VarChar).Value = txtCliente.Text;
            cargar.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = txtNombre.Text;
            cargar.Parameters.Add("@Apellido", SqlDbType.VarChar).Value = txtApellido.Text;
            cargar.Parameters.Add("@Documento", SqlDbType.Decimal).Value = txtDni.Text;
            SqlDataAdapter adapter = new SqlDataAdapter(cargar);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            conexion.Close();

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
            form.cargarDatos(decimal.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarClientes();
        }


        private void FormCliente_Load(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

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
                conexion.Open();
                SqlCommand cargar = new SqlCommand("OSNR.EliminarCliente", conexion);
                cargar.CommandType = CommandType.StoredProcedure;
                cargar.Parameters.Add("@Cliente", SqlDbType.Decimal).Value = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                cargar.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = Config.fecha;
                cargar.ExecuteNonQuery();
                conexion.Close();
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                MessageBox.Show("Cliente eliminado exitosamente");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCliente.Text = "";
            txtDni.Text = "";
        }

        private void lblNombre_Click(object sender, EventArgs e)
        {

        }

        private void lblApellido_Click(object sender, EventArgs e)
        {

        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {

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
