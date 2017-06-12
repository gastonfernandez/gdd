using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba;
using UberFrba.Mappings;

namespace autom
{
    public partial class AltaModVehiculo : Form
    {

        Int64 idAuto = 0;
        AbmAutomovil cv;
        BaseDeDatos db = new BaseDeDatos();
        private SqlConnection conexion = new SqlConnection(Config.strConnection);

        public AltaModVehiculo(Int64 idA, AbmAutomovil cveh)
        {
            InitializeComponent();
            dgvChofer.ReadOnly = false;
            LLenarComboMarca();
            llenarComboModelo(comboSelec(comboMarca).Value);
            LLenarComboTurno();
            LLenarListaChofer();
            llenarComboActivo();
            cv = cveh;
            if (idA != 0)
            {
                this.idAuto = idA;
                Object[] auto = recuperarVehiculoCompleto(idA);
                llenarCamposVista(auto);
            }
            

        }

        public Object[] recuperarVehiculoCompleto(Int64 id)
        {
            conexion.Open();
            Object[] auto = new Object[7];

            String query = "select veh_id, veh_id_modelo, veh_id_chofer, veh_patente, veh_licencia, veh_rodado, veh_habilitado ";
            query += "from OSNR.Vehiculo "; 
            query += "where veh_id =" + id;

            DataTable dt = db.select_query(query);

            DataRow row = dt.Rows[0];
                auto[0] = Convert.ToString(row["veh_id"]);
                auto[1] = Convert.ToString(row["veh_id_modelo"]);
                auto[2] = Convert.ToString(row["veh_id_chofer"]);
                auto[3] = Convert.ToString(row["veh_patente"]);
                auto[4] = Convert.ToString(row["veh_licencia"]);
                auto[5] = Convert.ToString(row["veh_rodado"]);
                auto[6] = Convert.ToString(row["veh_habilitado"]);

            conexion.Close();
            return auto;
        }

        public Int32 recuperarIdMarca(Int32 idModelo)
        {
            String query = "select distinct ma.mar_id ";
            query += "from OSNR.marca ma ";
            query += "join OSNR.modelo mo on mo.mod_id_marca = ma.mar_id ";
            query += "where mod_id = " + idModelo ;

            DataTable dt = db.select_query(query);
            DataRow row = dt.Rows[0];

            return Convert.ToInt32(row["mar_id"]);
        }

        public void llenarCamposVista(Object[] auto)
        {
            #region Marca y Modelo
            Int32 idMarca = recuperarIdMarca(Convert.ToInt32(auto[1]));
            foreach (Combo combo in (List<Combo>)comboMarca.DataSource)
            {
                if (combo.Value == idMarca)
                    comboMarca.SelectedItem = combo;
            }
            llenarComboModelo(comboSelec(comboMarca).Value);
            foreach (Combo combo in (List<Combo>)comboModelo.DataSource)
            {
                if (combo.Value == Convert.ToInt32(auto[1]))
                    comboModelo.SelectedItem = combo;
            }
            #endregion

            txtPatente.Text = auto[3].ToString();
            txtLicencia.Text = auto[4].ToString();
            txtRodado.Text = auto[5].ToString();
            if (auto[6].ToString() == "True")
                comboActivo.SelectedIndex = 1;
            else
                comboActivo.SelectedIndex = 0;

        }

        public void llenarComboActivo()
        {
            List<Combo> lista = new List<Combo>();
            lista.Add(new Combo("No", 0));
            lista.Add(new Combo("Si", 1));
            comboActivo.DisplayMember = "act_id";
            comboActivo.ValueMember = "act_nombre";
            comboActivo.DataSource = lista; 
        }

        public void LLenarComboMarca()
        {
            List<Combo> lista = new List<Combo>();
            DataTable dt = db.select_query("select distinct mar_id, mar_nombre from OSNR.marca");//todas las marcas posibles
            
            foreach (DataRow row in dt.Rows)
                lista.Add(new Combo(Convert.ToString(row["mar_nombre"]), Convert.ToInt32(row["mar_id"])));
            comboMarca.DisplayMember = "mar_id";
            comboMarca.ValueMember = "mar_nombre";
            comboMarca.SelectedValue = lista[0];
            comboMarca.DataSource = lista;
            
        }

        public void llenarComboModelo(Int64 idMarca)
        {
            List<Combo> lista = new List<Combo>();
            DataTable dt = db.select_query("select mo.mod_id, mo.mod_nombre from osnr.marca ma join osnr.modelo mo on ma.mar_id = mo.mod_id_marca where ma.mar_id = " + idMarca);
            foreach(DataRow row in dt.Rows)
                lista.Add(new Combo(Convert.ToString(row["mod_nombre"]), Convert.ToInt32(row["mod_id"])));
            comboModelo.DisplayMember = "mod_id";
            comboModelo.ValueMember = "mod_nombre";
            comboModelo.DataSource = lista;
        }

        private void comboMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                llenarComboModelo(comboSelec(comboMarca).Value);
            }
            catch
            { }
        }

        public void LLenarComboTurno()
        {

            List<Combo> lista = new List<Combo>();
            DataTable dt = db.select_query("select distinct tur_id, tur_descripcion from OSNR.turno");
            foreach (DataRow row in dt.Rows)
                lista.Add(new Combo(Convert.ToString(row["tur_descripcion"]), Convert.ToInt32(row["tur_id"])));
            comboTurno.DisplayMember = "tur_id";
            comboTurno.ValueMember = "tur_descripcion";
            comboTurno.DataSource = lista; 
        }

        public void LLenarListaChofer()
        {
            conexion.Open();

            String query = "select u.usu_nombre nombreChofer, u.usu_apellido apellidoChofer, c.cho_id idChofer from OSNR.usuario u join OSNR.chofer c on c.cho_id_usuario = u.usu_id order by 1";
            SqlDataAdapter daChoferes = new SqlDataAdapter(query, conexion);
            DataSet dsChoferes = new DataSet();
            daChoferes.Fill(dsChoferes, "chofer");
            dgvChofer.DataSource = dsChoferes;
            dgvChofer.DataMember = "chofer";

            conexion.Close();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.cv.Show();
            this.cv.BuscarVehiculos();
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (idAuto == 0)
                {
                    nuevoAutomovil();
                }
                else
                {
                    modificarAutomovil();
                }
                this.cv.Show();
                this.cv.BuscarVehiculos();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void modificarAutomovil()
        {
            validarDatos();

            String query = "update OSNR.vehiculo set ";
            query += "veh_id_modelo = " + recuperarIdModelo() + ", ";
            query += "veh_id_chofer = " + Convert.ToInt64(dgvChofer.SelectedRows[0].Cells[2].Value) + ", ";
            query += "veh_patente = '" + txtPatente.Text + "', ";
            query += "veh_licencia = '" + txtLicencia.Text + "', ";
            query += "veh_rodado = '" + txtRodado.Text + "', ";
            query += "veh_habilitado = " + comboActivo.SelectedIndex;
            query += " where veh_id = " + idAuto;

            db.query(query);

        }

        public void nuevoAutomovil()
        { 
            validarDatos();
            db.query("insert into OSNR.vehiculo values(" + recuperarIdModelo() + "," + Convert.ToInt64(dgvChofer.SelectedRows[0].Cells[2].Value.ToString()) + ",'" + txtPatente.Text + "','" + txtLicencia.Text + "','" + txtRodado.Text + "'," + comboSelec(comboActivo).Value.ToString() + ")");
            insertarVehiculoTurno();
        }

        public void insertarVehiculoTurno()
        {
            DataTable dt = db.select_query("select max(veh_id) as vehId from OSNR.vehiculo where veh_patente = '" + txtPatente.Text + "' and veh_habilitado = " + 1);
            DataRow row = dt.Rows[0];
            Int64 idVehiculo = Convert.ToInt64(row["vehId"]);
            db.query("insert into OSNR.VehiculoTurno values (" + idVehiculo + "," + comboSelec(comboTurno).Value.ToString() + ")");
        }

        public Int64 recuperarIdModelo()
        {
            String query = "select max(mod_id) as idModelo from OSNR.marca ma join OSNR.modelo mo on ma.mar_id = mo.mod_id_marca where ma.mar_id =" + comboSelec(comboMarca).Value.ToString() + " and mo.mod_id = " + comboSelec(comboModelo).Value.ToString();
            DataTable dt = db.select_query(query);
            DataRow row = dt.Rows[0];
            return Convert.ToInt64(row["idModelo"]);
        }

        public Combo comboSelec(ComboBox cBox)
        {
            return (Combo)cBox.SelectedValue;
        }


        public void validarDatos()
        { 
            if (String.IsNullOrEmpty(comboMarca.Text))
                throw new Exception("Debe seleccionar una Marca");
            
            if (String.IsNullOrEmpty(comboSelec(comboModelo).Value.ToString()))
                throw new Exception("Debe ingresar un Modelo");
            if (String.IsNullOrEmpty(txtPatente.Text))
                throw new Exception("Debe ingresar una Patente");
            if (String.IsNullOrEmpty(comboTurno.Text))
                throw new Exception("Debe seleccionar un Turno");
            if (dgvChofer.SelectedRows.Count == 0)
                throw new Exception("Debe seleccionar un Chofer");

            validarUnicidadChofer(Convert.ToInt64(dgvChofer.SelectedRows[0].Cells[2].Value.ToString()));
            validarPatente();
            validarMarcaModelo();
        }

        public void validarMarcaModelo()
        { 
            DataTable dt = db.select_query("select max(mod_id) from OSNR.marca ma join OSNR.modelo mo on ma.mar_id = mo.mod_id_marca where ma.mar_id =" + comboSelec(comboMarca).Value.ToString() + " and mo.mod_id = " + comboSelec(comboModelo).Value.ToString());
            if (dt.Rows.Count == 0)
               throw new Exception("El nombre de Modelo ingresado no existe, o no se encuentra relacionado con la Marca ingresada");
        }

        public void validarUnicidadChofer(Int64 idChofer)
        {
            String query = "select veh_id from OSNR.vehiculo v join OSNR.chofer c on v.veh_id_chofer = c.cho_id where veh_habilitado = 1 and c.cho_id = " + idChofer.ToString();
            if (idAuto != 0)
                query += " and veh_id <> " + idAuto;
            DataTable dt = db.select_query(query);
            if (dt.Rows.Count > 0)
                throw new Exception("El chofer seleccionado ya tiene asignado un vehiculo activo");           
        }

        public void validarPatente()
        {
            String query = "select veh_id from OSNR.vehiculo where veh_habilitado = 1 and veh_patente = '" + txtPatente.Text + "'";
            if (idAuto != 0)
                query += " and veh_id <> " + idAuto;
            DataTable dt = db.select_query(query);
            if (dt.Rows.Count > 0)
                throw new Exception("Ya existe un Automovil con la patente ingresada");   
        }

 

       


    }
}
