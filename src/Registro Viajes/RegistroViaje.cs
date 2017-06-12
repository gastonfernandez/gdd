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
        
        Validacion v = new Validacion();
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

            SqlDataAdapter daClientes = new SqlDataAdapter("select top 10 u.usu_dni as ChoferDni, u.usu_nombre as ChoferNombre, u.usu_apellido as ChoferApellido, u.usu_direccion as ChoferDireccion, u.usu_telefono as ChoferTelefono, u.usu_fecha_nacimiento as ChoferFechaNac ,	mar.mar_nombre as Marca,			m.mod_nombre as Modelo,			v.veh_patente as Patente,			t.tur_descripcion as Turno,			c.cho_id as id_chofer,			v.veh_id as id_vehiculo,			t.tur_id as id_turno  "+
	  " from OSNR.Usuario u	join OSNR.Chofer c on c.cho_id_usuario=u.usu_id "+
                " Join OSNR.Vehiculo v on v.veh_id_chofer = c.cho_id"+
		        " join OSNR.VehiculoTurno vt on vt.auttur_id_vehiculo=v.veh_id"+
		        " join OSNR.Turno t on t.tur_id=vt.auttur_id_turno"+
                " join OSNR.Modelo m on m.mod_id=v.veh_id_modelo"+
		         " Join OSNR.Marca mar on mar.mar_id=m.mod_id_marca"+
                " where t.tur_habilitado=1 and usu_dni like '" + Dni + "%' and usu_apellido like '" + Apellido + "%' and usu_nombre like '" + Nombre + "%'  ", conexion);
            DataSet dsClientes = new DataSet();
            daClientes.Fill(dsClientes, "chofer");
            dgvChofer.DataSource = dsClientes;
            dgvChofer.DataMember = "chofer";

            dgvChofer.Columns[10].Visible=false;
            dgvChofer.Columns[11].Visible = false;
            dgvChofer.Columns[12].Visible = false;
            conexion.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            recuperarClientes(tbApellidoCliente.Text, tbNombreCliente.Text, tbDniCliente.Text);
        }


        private void recuperarClientes(String Apellido, String Nombre, String Dni)
        {
            conexion.Open();

            SqlDataAdapter daClientes = new SqlDataAdapter("select top 10 u.usu_dni as ClienteDni, u.usu_nombre as ClienteNombre, u.usu_apellido as ClienteApellido, u.usu_direccion as ClienteDireccion, u.usu_telefono as ClienteTelefono, u.usu_fecha_nacimiento as ClienteFechaNacimiento, c.cli_id as id_Cliente from OSNR.Usuario u	join OSNR.Cliente c on c.cli_id_usuario=u.usu_id where usu_dni like '" + Dni + "%' and usu_apellido like '" + Apellido + "%' and usu_nombre like '" + Nombre + "%'  ", conexion);
            DataSet dsClientes = new DataSet();
            daClientes.Fill(dsClientes, "cliente");
            dgvCliente.DataSource = dsClientes;
            dgvCliente.DataMember = "cliente";
            dgvCliente.Columns[6].Visible = false;
        
            conexion.Close();

        }

      
        

        private void tbKM_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void tbDniCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void tbDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private Int32 idChofer=0;

        private Int32 idCliente=0;

        private Int32 idVehiculo=0;

        private Int32 idTurno=0;
        private Int32 cantKm=-1;
        private DateTime fechaDesde;
        private DateTime fechaHasta;

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    cantKm = Convert.ToInt32(tbKM.Text);
                }
                catch
                {
                    MessageBox.Show("La cantidad de Km debe ser completada");
                    return;
                }
                fechaDesde = dtFechaDesde.Value;
                fechaHasta = dtFechaHasta.Value;

                if (cantKm < 0)
                {
                    throw(new Exception ("la cantidad de km debe ser mayor a 0"));
                }

                if (dgvChofer.CurrentRow == null)
                    throw(new Exception("debe seleccionar al menos un chofer"));

                 if (dgvCliente.CurrentRow == null)
                    throw(new Exception("debe seleccionar al menos un cliente"));


                    foreach (DataGridViewCell item in dgvChofer.CurrentRow.Cells)
                    {
                        if (item.ColumnIndex == 10)
                            idChofer = Convert.ToInt32(item.Value);
                        if (item.ColumnIndex == 11)
                            idVehiculo = Convert.ToInt32(item.Value);
                        if (item.ColumnIndex == 12)
                            idTurno = Convert.ToInt32(item.Value);
                    }

                foreach (DataGridViewCell item in dgvCliente.CurrentRow.Cells)
                {
                    if (item.ColumnIndex == 6)
                        idCliente = Convert.ToInt32(item.Value);

                }


                if (idChofer != 0 && idCliente != 0 && idTurno != 0 && idVehiculo != 0)
                {

                    #region ValidarCliente con viaje registrado en fecha y hora (chofer tambien)


                    Dictionary<String, DbTypedValue> camposchofer = new Dictionary<string, DbTypedValue>();
                    camposchofer.Add("idChofer", new DbTypedValue(idChofer.ToString(), SqlDbType.BigInt));
                    camposchofer.Add("fechaDesde", new DbTypedValue(fechaDesde, SqlDbType.DateTime));
                    camposchofer.Add("fechaHasta", new DbTypedValue(fechaHasta, SqlDbType.DateTime));
                    Dictionary<int, String> errormsgcho = new Dictionary<int, string>();
                    DataGridView rtaChofer = new DataGridView();
                    rtaChofer.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.BuscarViajesChofer", camposchofer, errormsgcho);

                   
                    //no esta funcionando el sp hay que validarlo
                    if (rtaChofer.RowCount != 0)
                        throw new Exception("No es posible cargar un viaje para el chofer ya que dispone uno cargado");

                    Dictionary<String, DbTypedValue> camposcliente = new Dictionary<string, DbTypedValue>();
                    camposcliente.Add("idCliente", new DbTypedValue(idChofer.ToString(), SqlDbType.BigInt));
                    camposcliente.Add("fechaDesde", new DbTypedValue(fechaDesde.ToString(), SqlDbType.DateTime));
                    camposcliente.Add("fechaHasta", new DbTypedValue(fechaHasta.ToString(), SqlDbType.DateTime));
                    Dictionary<int, String> errormsgcli = new Dictionary<int, string>();
                    DataGridView rtacli = new DataGridView();
                    rtacli.DataSource = new BaseDeDatos().ExecSPAndGetData("OSNR.BuscarViajesCliente", camposcliente, errormsgcli);

                    if (rtacli.RowCount !=0)
                        throw new Exception("No es posible cargar un viaje para el cliente ya que dispone uno cargado");

                    

                    #endregion

                    #region Ejecutar SP registroViaje
                    Dictionary<String, DbTypedValue> campos = new Dictionary<string, DbTypedValue>();
                    campos.Add("idChofer", new DbTypedValue(idChofer.ToString(), SqlDbType.BigInt));
                    campos.Add("idCliente", new DbTypedValue(idCliente.ToString(), SqlDbType.BigInt));
                    campos.Add("idTurno", new DbTypedValue(idTurno.ToString(), SqlDbType.BigInt));
                    campos.Add("idVehiculo", new DbTypedValue(idVehiculo.ToString(), SqlDbType.BigInt));
                    campos.Add("fechaDesde", new DbTypedValue(fechaDesde.ToString(), SqlDbType.DateTime));
                    campos.Add("fechaHasta", new DbTypedValue(fechaHasta.ToString(), SqlDbType.DateTime));
                    campos.Add("cantKm", new DbTypedValue(cantKm.ToString(), SqlDbType.Int));

                    Dictionary<int, String> errormsg = new Dictionary<int, string>();
                    new BaseDeDatos().ExecSP("OSNR.RegistrarViaje", campos, errormsg);
                    if (errormsg.Count == 0)
                        MessageBox.Show("El viaje ha sido registrado correctamente");
                    #endregion
                }
                else
                    throw new Exception("Hubo un problema al realizar la operacion. Pongase en contacto con el administrador");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

       
        



    }
}
