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

namespace UberFrba.Abm_Chofer
{
    public partial class AbmChofer : Form
    {
        SqlConnection conexion;
        Validacion v = new Validacion();

        public string idChoferSeleccionado = null;

        public AbmChofer(Boolean useAsSearch)
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
            cargarChoferes();
        }

        private void cargarChoferes()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("nombre", new DbTypedValue(txtNombre.Text, SqlDbType.VarChar));
            campos.Add("apellido", new DbTypedValue(txtApellido.Text, SqlDbType.VarChar));
            campos.Add("dni", new DbTypedValue(txtDni.Text, SqlDbType.VarChar));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView1.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.BuscarChoferes", campos, errorMensaje);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                EditarChofer form = new EditarChofer();
                form.Tag = "Editar";
                form.cargarDatos(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    cargarChoferes();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un Chofer para editar");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarChoferes();
        }

        private void btnModificarHabilitacion_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
                String ChoferId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                campos.Add("ChoferId", new DbTypedValue(ChoferId, SqlDbType.Decimal));
                Boolean habilitado = Convert.ToBoolean(dataGridView1.SelectedRows[0].Cells[7].Value.ToString());

                if (habilitado)
                {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea deshabilitar al Chofer?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.DeshabilitarChofer", campos, errorMensaje);

                        MessageBox.Show("Chofer deshabilitado exitosamente");
                        cargarChoferes();
                    }
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea habilitar al Chofer?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.HabilitarChofer", campos, errorMensaje);

                        MessageBox.Show("Chofer habilitado exitosamente");
                        cargarChoferes();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un Chofer para deshabilitar");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
            cargarChoferes();
        }

        private void txtChofer_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show("Debe seleccionar un chofer");
                return;
            }
            this.idChoferSeleccionado = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            this.Close();
        }

    }
}
