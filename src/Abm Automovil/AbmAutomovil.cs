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
    public partial class AbmAutomovil : Form
    {
        private SqlConnection conexion = new SqlConnection(Config.strConnection);

        public AbmAutomovil()
        {
            InitializeComponent();
            LlenarComboMarca();
            BuscarVehiculos();
            
        }

        public void LlenarComboMarca()
        {
            List<Combo> lista = new List<Combo>();
            BaseDeDatos db = new BaseDeDatos();
            DataTable dt = db.select_query("select distinct mar_id, mar_nombre from OSNR.marca");//todas las marcas posibles

            lista.Add(new Combo("Todas", 0));

            foreach (DataRow row in dt.Rows)
                lista.Add(new Combo(Convert.ToString(row["mar_nombre"]), Convert.ToInt32(row["mar_id"])));
            comboMarca.DisplayMember = "mar_id";
            comboMarca.ValueMember = "mar_nombre";
            comboMarca.DataSource = lista;
            comboMarca.SelectedItem = lista[0];
        }

        public void BuscarVehiculos()
        {
            //listaConsulta.Items.Clear();
            BaseDeDatos db = new BaseDeDatos();//fijarse si no hay que sacarlo
            conexion.Open();

            Combo marca = (Combo)comboMarca.SelectedItem;

            String query = "select distinct v.veh_id,v.veh_patente,mo.mod_nombre, ma.mar_nombre, u.usu_nombre,v.veh_habilitado ";
            query += "from OSNR.vehiculo v ";
            query += "join OSNR.chofer ch on ch.cho_id = v.veh_id_chofer ";
            query += "join OSNR.usuario u on u.usu_id = ch.cho_id_usuario ";
            query += "join OSNR.modelo mo on mo.mod_id = v.veh_id_modelo ";
            query += "join OSNR.marca ma on ma.mar_id = mo.mod_id_marca ";
            query += "where ";
            query += "mo.mod_nombre like '%" + txtModelo.Text + "%' ";
            query += "and (ma.mar_id= " + marca.Value + " or 0=" + marca.Value + ")";
            query += "and v.veh_patente like '%" + txtPatente.Text + "%' ";
            query += "and u.usu_nombre like '%" + txtChofer.Text + "%' ";
            query += "order by veh_habilitado desc, 1 ";


            SqlDataAdapter daVehiculos = new SqlDataAdapter(query, conexion);
            DataSet dsVehiculos = new DataSet();
            daVehiculos.Fill(dsVehiculos, "vehiculo");
            dgvVehiculos.DataSource = dsVehiculos;
            dgvVehiculos.DataMember = "vehiculo";

            //foreach (DataRow row in dt.Rows)
            //{
            //    ListViewItem item = new ListViewItem(Convert.ToString(row["veh_id"]));//no deberia mostrarse
            //    item.SubItems.Add(Convert.ToString(row["veh_patente"]));
            //    item.SubItems.Add(Convert.ToString(row["mar_nombre"]));
            //    item.SubItems.Add(Convert.ToString(row["mod_nombre"]));
            //    item.SubItems.Add(Convert.ToString(row["usu_nombre"]));
            //    item.SubItems.Add(Convert.ToByte(row["veh_habilitado"]) == 1 ? "Activo" : "Inactivo");

            //    listaConsulta.Items.Add(item);
            //}
            conexion.Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarVehiculos();
        }

        private void altaVehiculo_Click(object sender, EventArgs e)
        {
            ListViewItem itm = null;
            AltaModVehiculo amv = new AltaModVehiculo(itm, this);
            amv.Show();
            this.Hide();   
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvVehiculos.SelectedRows.Count == 0)
                MessageBox.Show("Debe seleccionar un item de la lista");

            foreach (DataGridViewRow item in dgvVehiculos.SelectedRows)
            {
                if ((Boolean)item.Cells[5].Value == true)
                    eliminarVehiculo(Convert.ToInt64(item.Cells[0].Value));
                else 
                    MessageBox.Show("El Automovil ya se encuentra DESHABILITADO");
            }
            BuscarVehiculos();
        }

        public void eliminarVehiculo(Int64 id) 
        {
            try
            {
                BaseDeDatos db = new BaseDeDatos();
                db.query("update OSNR.vehiculo set veh_habilitado = 0 where veh_id = " + id);
            }
            catch
            {
                MessageBox.Show("El Automovil no pudo eliminarse correctamente. Intentelo nuevamente.");
            }
        }

    }

}
