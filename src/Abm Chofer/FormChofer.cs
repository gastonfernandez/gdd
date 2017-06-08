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

namespace UberFrba.Abm_Chofer
{
    public partial class FormChofer : Form
    {
        
        SqlConnection conexion;
        Validacion v = new Validacion();

        public FormChofer()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Config.strConnection);
        }

        private void cargarChoferes()
        {
            conexion.Open();
            SqlCommand cargar = new SqlCommand("OSNR.CargarChoferes", conexion);
            cargar.CommandType = CommandType.StoredProcedure;
            cargar.Parameters.Add("@Chofer", SqlDbType.VarChar).Value = txtChofer.Text;
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
            EditarChofer form = new EditarChofer();
            form.Tag = "Editar";
            form.cargarDatos(decimal.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarChoferes();
        }


        private void FormChofer_Load(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarChoferes();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Está seguro que desea eliminar al chofer", "Uber", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                conexion.Open();
                SqlCommand cargar = new SqlCommand("OSNR.EliminarChofer", conexion);
                cargar.CommandType = CommandType.StoredProcedure;
                cargar.Parameters.Add("@Chofer", SqlDbType.Decimal).Value = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                cargar.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = Config.fecha;
                cargar.ExecuteNonQuery();
                conexion.Close();
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                MessageBox.Show("Chofer eliminado exitosamente");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtChofer.Text = "";
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

        private void txtChofer_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            habilitarBotones();
        }

        private void btnAñadir_Click(object sender, EventArgs e)
        {
            EditarChofer form = new EditarChofer();
            form.Tag = "Agregar";
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarChoferes();
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
