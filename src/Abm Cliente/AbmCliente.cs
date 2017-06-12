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
        SqlConnection conexion;
        Validacion v = new Validacion();
        public string idClienteSeleccionado = null;

        public AbmCliente(Boolean useAsSearch)
        {
            InitializeComponent();
            if (useAsSearch)
            {
                this.btnAñadir.Visible = false;
                this.btnEditar.Visible = false;
                this.btnModificarHabilitacion.Visible = false;
                this.btnSeleccionar.Visible = true;
            }
            else
            {
                this.btnAñadir.Visible = true;
                this.btnEditar.Visible = true;
                this.btnModificarHabilitacion.Visible = true;
                this.btnSeleccionar.Visible = false;
            }
            conexion = new SqlConnection(@Config.strConnection);
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
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                FormEditarCliente form = new FormEditarCliente();
                form.Tag = "Editar";
                form.cargarDatos(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    cargarClientes();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un cliente para editar");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarClientes();
        }

        private void btnModificarHabilitacion_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
                String clienteId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                campos.Add("clienteId", new DbTypedValue(clienteId, SqlDbType.Decimal));
                Boolean habilitado = Convert.ToBoolean(dataGridView1.SelectedRows[0].Cells[7].Value.ToString());

                if(habilitado)
                {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea deshabilitar al Cliente?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.DeshabilitarCliente", campos, errorMensaje);

                        MessageBox.Show("Cliente deshabilitado exitosamente");
                        cargarClientes();
                    } 
                } else {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea habilitar al Cliente?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.HabilitarCliente", campos, errorMensaje);

                        MessageBox.Show("Cliente habilitado exitosamente");
                        cargarClientes();
                    }                   
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un cliente para deshabilitar");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
            cargarClientes();
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
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

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show("Debe seleccionar un cliente");
                return;
            }
            this.idClienteSeleccionado = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            this.Close();
        }

    }
}
