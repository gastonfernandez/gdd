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
    public partial class EditarChofer : Form
    {
        Validacion v = new Validacion();
        SqlConnection conexion;
        String ChoferId = null;
        List<EditarChofer> afiliadosChoferes = new List<EditarChofer>();

        public EditarChofer()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Config.strConnection);
            dtpNacimiento.Value = Config.fecha;
        }

        public void cargarDatos(String numeroChofer)
        {
            ChoferId = numeroChofer;

            conexion.Open();

            String query = "SELECT * FROM OSNR.Usuario JOIN OSNR.Chofer ON usu_id = cho_id_usuario WHERE cho_id = '" + numeroChofer + "'";

            SqlCommand listar = new SqlCommand(query, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = listar;
            adapter.Fill(tabla);

            //txtNombre.Enabled = false;
            //txtApellido.Enabled = false;
            //txtDocumento.Enabled = false;
            //dtpNacimiento.Enabled = false;

            txtNombre.Text = tabla.Rows[0]["usu_nombre"].ToString();
            txtApellido.Text = tabla.Rows[0]["usu_apellido"].ToString();
            txtDocumento.Text = tabla.Rows[0]["usu_dni"].ToString();

            txtDomicilio.Text = tabla.Rows[0]["usu_direccion"].ToString();

            txtTelefono.Text = tabla.Rows[0]["usu_telefono"].ToString();
            txtEmail.Text = tabla.Rows[0]["usu_mail"].ToString();
            dtpNacimiento.Text = tabla.Rows[0]["usu_fecha_nacimiento"].ToString();

            conexion.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (camposCompletos())
            {
                if (existeTelefono())
                {
                    MessageBox.Show("El telefono ya se encuentra en uso por otro usuario");
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    guardarDatos();
                    MessageBox.Show("Datos guardados correctamente!");
                    this.Close();
                }
            }
        }

        private bool existeTelefono()
        {
            conexion.Open();
            String query = "SELECT usu_id FROM OSNR.Usuario JOIN OSNR.Chofer ON usu_id = cho_id_usuario WHERE usu_telefono = '" + txtTelefono.Text + "' AND cho_id <> '" + ChoferId + "'";

            SqlCommand listar = new SqlCommand(query, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = listar;
            adapter.Fill(tabla);
            conexion.Close();
            if (tabla.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void guardarDatos()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("ChoferId", new DbTypedValue(ChoferId, SqlDbType.Decimal));
            campos.Add("Nombre", new DbTypedValue(txtNombre.Text, SqlDbType.VarChar));
            campos.Add("Apellido", new DbTypedValue(txtApellido.Text, SqlDbType.VarChar));
            campos.Add("Dni", new DbTypedValue(txtDocumento.Text, SqlDbType.Decimal));
            campos.Add("Direccion", new DbTypedValue(txtDomicilio.Text, SqlDbType.VarChar));
            campos.Add("Telefono", new DbTypedValue(txtTelefono.Text, SqlDbType.Decimal));
            campos.Add("Email", new DbTypedValue(txtEmail.Text, SqlDbType.VarChar));
            campos.Add("FechaNac", new DbTypedValue(dtpNacimiento.Text, SqlDbType.DateTime));

            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            new BaseDeDatos().ExecSP("OSNR.ModificarOCrearChofer", campos, errorMensaje);
        }

        private bool camposCompletos()
        {
            if (txtNombre.Text == "")
            {
                MessageBox.Show("Complete el nombre");
            }
            else if (txtApellido.Text == "")
            {
                MessageBox.Show("Complete el apellido");
            }
            else if (txtDocumento.Text == "")
            {
                MessageBox.Show("Complete el documento");
            }
            else if (!esNumerico(txtDocumento.Text))
            {
                MessageBox.Show("El documento debe contener solo numeros");
            }
            else if (txtDomicilio.Text == "")
            {
                MessageBox.Show("Complete el domicilio");
            }
            else if (txtTelefono.Text == "")
            {
                MessageBox.Show("Complete el telefono");
            }
            else if (txtEmail.Text == "")
            {
                MessageBox.Show("Complete el email");
            }
            else if (!esNumerico(txtTelefono.Text))
            {
                MessageBox.Show("El telefono debe contener solo numeros");
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool esNumerico(String cadena)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(cadena, @"^\d+$");
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

        private void txtDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }
    }
}
