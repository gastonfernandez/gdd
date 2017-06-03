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
    public partial class FormEditarCliente : Form
    {
        Validacion v = new Validacion();
        SqlConnection conexion;
        decimal cliente;
        List<FormEditarCliente> afiliadosClientes = new List<FormEditarCliente>();

        public FormEditarCliente()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Configuraciones.datosConexion);
            conexion.Open();

            String query = "SELECT Plan_Codigo, Plan_Descripcion FROM CHAMBA.Planes";

            SqlCommand listar = new SqlCommand(query, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = listar;
            adapter.Fill(tabla);

            conexion.Close();

            dtpNacimiento.Value = Configuraciones.fecha;
        }

        public void cargarDatos(decimal numeroCliente)
        {
            cliente = numeroCliente;

            conexion.Open();

            String query = "SELECT * FROM OSNR.Usuario JOIN OSNR.Cliente ON usu_id = cli_id_usuario WHERE cli_id = '" + numeroCliente + "'";

            SqlCommand listar = new SqlCommand(query, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = listar;
            adapter.Fill(tabla);

            txtNombre.Enabled = false;
            txtApellido.Enabled = false;
            cboTipoDocumento.Enabled = false;
            txtDocumento.Enabled = false;
            dtpNacimiento.Enabled = false;
            
            txtNombre.Text = tabla.Rows[0]["usu_nombre"].ToString();
            txtApellido.Text = tabla.Rows[0]["usu_apellido"].ToString();
            txtDocumento.Text = tabla.Rows[0]["usu_dni"].ToString();
            txtCalle.Text = tabla.Rows[0]["usu_direccion"].ToString();

            txtTelefono.Text = tabla.Rows[0]["usu_telefono"].ToString();
            txtEmail.Text = tabla.Rows[0]["usu_mail"].ToString();
            dtpNacimiento.Text = tabla.Rows[0]["usua_fecha_nacimiento"].ToString();

            conexion.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (camposCompletos())
            {

                if (existeEmail())
                {
                    MessageBox.Show("El email ya se encuentra en uso por otro usuario");
                }
                else
                {

                    this.DialogResult = DialogResult.OK;
                    if (this.Tag.ToString() != "Hijo" && this.Tag.ToString() != "Conyuge")
                    {
                        guardarDatos();
                        MessageBox.Show("Datos guardados exitosamente");
                        this.Close();
                    }
                }

            }
        }

        private bool existeEmail()
        {
            conexion.Open();
            String query = "SELECT usu_id FROM OSNR.Usuario JOIN OSNR.Cliente ON usu_id = cli_id_usuario WHERE usu_mail = '" + txtEmail.Text + "' AND cli_id <> '" + cliente + "'";

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

        public SqlCommand generarComandoSQL()
        {
            SqlCommand guardar;
            guardar = new SqlCommand();
            guardar.CommandType = CommandType.StoredProcedure;

            guardar.CommandText = "OSNR.ModificarCliente";

            guardar.Parameters.Add("@Cliente", SqlDbType.Decimal).Value = cliente;
            guardar.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = txtNombre.Text;
            guardar.Parameters.Add("@Apellido", SqlDbType.VarChar).Value = txtApellido.Text;
            guardar.Parameters.Add("@Documento", SqlDbType.Decimal).Value = txtDocumento.Text;
            guardar.Parameters.Add("@Domicilio", SqlDbType.VarChar).Value = txtDomicilio.Text;
            guardar.Parameters.Add("@Telefono", SqlDbType.Decimal).Value = txtTelefono.Text;
            guardar.Parameters.Add("@Email", SqlDbType.VarChar).Value = txtEmail.Text;
            guardar.Parameters.Add("@FechaNac", SqlDbType.DateTime).Value = dtpNacimiento.Text;

            return guardar;
        }

        private void guardarDatos()
        {
            conexion.Open();


            if (this.Tag.ToString() == "Agregar")
            {
                SqlCommand nuevoIdPaciente = new SqlCommand("OSNR.ObtenerNuevoIdCliente", conexion);
                nuevoIdCliente.CommandType = CommandType.StoredProcedure;

                var nuevoId = nuevoIdCliente.Parameters.Add("@id", SqlDbType.Decimal);
                nuevoId.Direction = ParameterDirection.Output;
                SqlDataReader dataId = nuevoIdCliente.ExecuteReader();
                dataId.Close();
                cliente = decimal.Parse(nuevoId.Value.ToString());
            }

            SqlTransaction transaccion;

            transaccion = conexion.BeginTransaction("Transaccion");

            SqlCommand comando = generarComandoSQL();
            comando.Connection = conexion;
            comando.Transaction = transaccion;


            if (planCambiado != null)
            {
                SqlCommand comandoCambioPlan = planCambiado.generarComandoSQL();
                comandoCambioPlan.Connection = conexion;
                comandoCambioPlan.Transaction = transaccion;
                comandoCambioPlan.ExecuteNonQuery();
            }

            comando.ExecuteNonQuery();

            transaccion.Commit();
            conexion.Close();
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
            else if (!esNumerico(txtTelefono.Text))
            {
                MessageBox.Show("El telefono debe contener solo numeros");
            }
            else if (txtEmail.Text == "")
            {
                MessageBox.Show("Complete el email");
            }
                //faltan mas
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloLetras(e);
        }

        private void txtDocumento_KeyUp(object sender, KeyEventArgs e)
        {

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
