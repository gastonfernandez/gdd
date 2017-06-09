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
using System.Data;
using System.Data.SqlClient;

namespace UberFrba.Registro_Viajes
{
    public partial class RegistroViaje : Form
    {
        public RegistroViaje()
        {
            InitializeComponent();
        }
        private SqlConnection conexion = new SqlConnection(Config.strConnection);


        private void RegistroViaje_Load(object sender, EventArgs e)
        {
         

        }

        private void tbTelefonoCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbApellidoCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void btCargarChofer_Click(object sender, EventArgs e)
        {
            recuperarChoferes(tbApellido.Text, tbNombre.Text, tbDni.Text);
        }

        private void recuperarChoferes(String Apellido, String Nombre, String Dni)
        {
            conexion.Open();

            SqlDataAdapter daClientes = new SqlDataAdapter("select top 10 u.usu_dni, u.usu_nombre, u.usu_apellido, u.usu_direccion, u.usu_telefono, u.usu_fecha_nacimiento from OSNR.Usuario u	join OSNR.Chofer c on c.cho_id_usuario=u.usu_id where usu_dni like '"+Dni+"%' and usu_apellido like '"+Apellido+"%' and usu_nombre like '"+Nombre+"%'  ", conexion);
            DataSet dsClientes = new DataSet();
            daClientes.Fill(dsClientes, "chofer");
            dgvChofer.DataSource = dsClientes;
            dgvChofer.DataMember = "chofer";

            conexion.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            recuperarClientes(tbApellidoCliente.Text, tbNombreCliente.Text, tbDniCliente.Text);
        }


        private void recuperarClientes(String Apellido, String Nombre, String Dni)
        {
            conexion.Open();

            SqlDataAdapter daClientes = new SqlDataAdapter("select top 10 u.usu_dni, u.usu_nombre, u.usu_apellido, u.usu_direccion, u.usu_telefono, u.usu_fecha_nacimiento from OSNR.Usuario u	join OSNR.Cliente c on c.cli_id_usuario=u.usu_id where usu_dni like '" + Dni + "%' and usu_apellido like '" + Apellido + "%' and usu_nombre like '" + Nombre + "%'  ", conexion);
            DataSet dsClientes = new DataSet();
            daClientes.Fill(dsClientes, "cliente");
            dgvCliente.DataSource = dsClientes;
            dgvCliente.DataMember = "cliente";

            conexion.Close();

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }



    }
}
