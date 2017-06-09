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

        ListViewItem item;
        AbmAutomovil cv;
        BaseDeDatos db = new BaseDeDatos();
        private SqlConnection conexion = new SqlConnection(Config.strConnection);

        public AltaModVehiculo(ListViewItem itm, AbmAutomovil cveh)
        {
            this.item = itm;
            this.cv = cveh;
            if (this.item != null)
                llenarCamposVista();
            InitializeComponent();
            LLenarComboMarca();
            llenarComboModelo(comboSelec(comboMarca).Value);
            LLenarComboTurno();
            LLenarListaChofer();
            llenarComboActivo();

        }

        public void llenarCamposVista()
        {
            //String query = "select distinct veh_id_modelo,veh_id_chofer, veh_patente,veh_licencia,veh_rodado,veh_habilitado, vt.auttur_id_vehiculo ";
            //query += "from OSNR.vehiculo v join OSNR.vehiculoturno vt on vt.auttur_id_vehiculo = v.veh_id ";
            //query += "where veh_id =" + this.item.Text;
            //DataTable dtAuto = db.select_query(query);
            //DataTable dtAutoTurno = db.select_query("select auttur_id_turno from OSNR.vehiculo v join OSNR.vehiculoturno vt on vt.auttur_id_vehiculo = v.veh_id where v.veh_id = " + this.item.Text);

            //DataRow auto = dtAuto.Rows[0];
            //DataRow autoTurno = dtAutoTurno.Rows[0];

            //txtModelo.Text = item.SubItems[2].Text;
            ////comboMarca.seleccionado = al id de marca relacionado con el modelo recuperado
            //txtPatente.Text = item.SubItems[0].Text;
            //txtLicencia.Text = Convert.ToString(auto["veh_licencia"]);
            //txtRodado.Text = Convert.ToString(auto["veh_rodado"]);
            ////comboActivo.seleccionado: id del combo = al valor que traiga Convert.ToByte(auto["veh_habilitado"]);
            ////comboTurno.seleccionado: id del combo = al valor que traiga Convert.ToInt64(autoTurno["auttur_id_turno"]);
            ////chofer: Id de la lista seleccionado = Convert.ToByte(auto["veh_id_chofer"]);
        }

        public void llenarComboActivo()
        {
            List<Combo> lista = new List<Combo>();
            lista.Add(new Combo("Si", 1));
            lista.Add(new Combo("No", 0));
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
            //lista.Add(new Combo("Chevrolet", 1));//sacar
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
            llenarComboModelo(comboSelec(comboMarca).Value);
        }

        public void LLenarComboTurno()
        {

            List<Combo> lista = new List<Combo>();
            DataTable dt = db.select_query("select distinct tur_id, tur_descripcion from OSNR.turno");
            foreach (DataRow row in dt.Rows)
                lista.Add(new Combo(Convert.ToString(row["tur_descripcion"]), Convert.ToInt32(row["tur_id"])));
            //lista.Add(new Combo("Noche", 1));//sacar
            comboTurno.DisplayMember = "tur_id";
            comboTurno.ValueMember = "tur_descripcion";
            comboTurno.DataSource = lista; 
        }

        public void LLenarListaChofer()
        {
            conexion.Open();

            String query = "select u.usu_nombre, u.usu_apellido, c.cho_id from OSNR.usuario u join OSNR.chofer c on c.cho_id_usuario = u.usu_id";
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
                if (item == null)
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

            String query = "update vehiculo set ";
            query += "veh_id_modelo = " + recuperarIdModelo() + ", ";
            query += "veh_id_chofer = " + Convert.ToInt64(dgvChofer.SelectedRows[0].Cells[2].Value.ToString()) + ", ";
            query += "veh_patente = " + txtPatente.Text + ", ";
            query += "veh_licencia = " + txtLicencia.Text + ", ";
            query += "veh_rodado = " + txtRodado.Text + ", ";
            query += "veh_habilitado = " + comboActivo.ValueMember.ToString();
            query += " where veh_id = " + item.Text;

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
            DataTable dt = db.select_query("select max(mod_id) as idModelo from OSNR.marca ma join OSNR.modelo mo on ma.mar_id = mo.mod_id_marca where ma.mar_id =" + comboSelec(comboMarca).Value.ToString() + " and mo.mod_id = " + Convert.ToInt64(dgvChofer.SelectedRows[0].Cells[2].Value.ToString()));
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
            if (item != null)
                query += " and veh_id <> " + item.Text;
            DataTable dt = db.select_query(query);
            if (dt.Rows.Count > 0)
                throw new Exception("El chofer seleccionado ya tiene asignado un vehiculo activo");           
        }

        public void validarPatente()
        {
            String query = "select veh_id from OSNR.vehiculo where veh_habilitado = 1 and veh_patente = '" + txtPatente.Text + "'";
            if (item != null)
                query += " and veh_id <> " + item.Text;
            DataTable dt = db.select_query(query);
            if (dt.Rows.Count > 0)
                throw new Exception("Ya existe un Automovil con la patente ingresada");   
        }

 

       


    }
}
