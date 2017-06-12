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

namespace UberFrba.Abm_Turno
{
    public partial class AbmTurno : Form
    {
        SqlConnection conexion;
        Validacion v = new Validacion();

        public AbmTurno()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Config.strConnection);
            cargarTurnos();
        }

        private void cargarTurnos()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("descripcion", new DbTypedValue(txtDescripcion.Text, SqlDbType.VarChar));
            
            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            dataGridView1.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.BuscarTurnos", campos, errorMensaje);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                FormEditarTurno form = new FormEditarTurno();
                form.Tag = "Editar";
                form.cargarDatos(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    cargarTurnos();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un Turno para editar");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarTurnos();
        }

        private void btnModificarHabilitacion_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
                String TurnoId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                campos.Add("TurnoId", new DbTypedValue(TurnoId, SqlDbType.Decimal));
                Boolean habilitado = Convert.ToBoolean(dataGridView1.SelectedRows[0].Cells[6].Value.ToString());

                if (habilitado)
                {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea deshabilitar al Turno?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.DeshabilitarTurno", campos, errorMensaje);

                        MessageBox.Show("Turno deshabilitado exitosamente");
                        cargarTurnos();
                    }
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("¿Está seguro que desea habilitar al Turno?", "Uber", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
                        new BaseDeDatos().ExecSP("OSNR.HabilitarTurno", campos, errorMensaje);

                        MessageBox.Show("Turno habilitado exitosamente");
                        cargarTurnos();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un Turno para deshabilitar");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDescripcion.Text = "";
            cargarTurnos();
        }

        private void txtTurno_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void btnAñadir_Click(object sender, EventArgs e)
        {
            FormEditarTurno form = new FormEditarTurno();
            form.Tag = "Agregar";
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                cargarTurnos();
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

    }
}
